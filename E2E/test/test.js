var fs = require('fs');
var chakram = require('chakram');
var expect = chakram.expect;
var spawn = require('child_process').spawn;

var schemaFile = 'schema.json';

describe('Test suite',
    function () {
        const schema = JSON.parse(fs.readFileSync(schemaFile));
        it('should return the schema JSON on index page', function () {
            return chakram.get('/')
                .then(function (response) {
                    expect(response).to.have.status(200);
                    expect(response).to.comprise.of.json(schema);
                });
        });

        it('should save entities and make snapshots', function () {
            var key = 1;
            var value = {
                foo: 1337,
                bar: 'hello'
            };
            var saveRequest = {
                makeSnapshot: true,
                key: key,
                value: value
            };
            var expectedEntity = {
                key: key,
                value: value
            };
            return chakram.post('/entities', saveRequest)
                .then(function (response) {
                    // Step 1 - save a new entity
                    expect(response).to.have.status(200);
                    return chakram.get('/entities/' + key);
                })
                .then(function (response) {
                    // Step 2 - check if the entity was successfully saved
                    expect(response).to.have.status(200);
                    expect(response).to.comprise.of.json(expectedEntity);
                    return chakram.get('/snapshots/' + key);
                })
                .then(function (response) {
                    // Step 3 - check if a snapshot of an entity was created
                    expect(response).to.have.status(200);
                    expect(response).to.comprise.of.json([{ state: expectedEntity }]);
                });
        });

        it('should check for existence and allow deletion', function () {
            var key = 2;
            var value = {
                foo: 0,
                bar: 'delete me'
            };
            var saveRequest = {
                makeSnapshot: true,
                key: key,
                value: value
            };

            return chakram.post('/entities', saveRequest)
                .then(function (response) {
                    // Step 1 - save a new entity
                    expect(response).to.have.status(200);
                    return chakram.head('/entities/' + key);
                })
                .then(function (response) {
                    // Step 2 - check if saved entity exists
                    expect(response).to.have.status(200);
                    return chakram.delete('/entities/' + key);
                })
                .then(function (response) {
                    // Step 3 - delete the entity
                    return chakram.head('/entities/' + key);
                })
                .then(function (response) {
                    // Step 4 - check if the entity has disappeared
                    expect(response).to.have.status(404);
                });
        });

        it('should return all keys and entities if requested', function () {
            var keys = [3, 4, 5];
            var entities = keys.map(function (item, index, array) {
                return {
                    key: item,
                    value: {
                        foo: item,
                        bar: 'test'
                    }
                }
            });

            // Step 1 - save some entities to database
            var requests = entities.reduce(function (accumulator, entity, index, arr) {
                console.dir(index);
                if (index === 1) {
                    accumulator = chakram.post('/entities', Object.assign({}, accumulator));
                }
                return accumulator.then(function (response) {
                    expect(response).to.have.status(200);
                    return chakram.post('/entities', Object.assign({}, entity));
                });
            });

            return requests
                .then(function (response) {
                    expect(response).to.have.status(200);
                    // Step 2 - get the keys list
                    return chakram.get('/keys');
                })
                .then(function (response) {
                    expect(response).to.have.status(200);
                    expect(response).to.comprise.of.json(keys);
                    // Step 3 - get all entities
                    return chakram.get('/entities');
                })
                .then(function (response) {
                    expect(response).to.have.status(200);
                    expect(response).to.comprise.of.json(entities);
                });
        });



        /* Setup and teardown */
        var dbms = null;
        var fd = -1;

        before(function (done) {
            fd = fs.openSync(schemaFile, 'r');
            dbms = spawn('dotnet', [
                'run',
                '--no-build',
                '--project',
                '../Standalone/Standalone.csproj',
                '--',
                '--loadSchemaFromStdIn',
                '--store',
                'InMemory'
            ], options = {
                detached: true,
                stdio: [fd, 'pipe', process.stderr]
            });
            dbms.on('exit', function (code, signal) {
                if (code !== 0) throw new Error("Could not start DBMS");
            });
            dbms.stdout.on('data', function (data) {
                var line = String.fromCharCode.apply(String, data);
                // Extract server listening URI if available
                var URIs = line.match(/(https?:\/\/[^\s]+[\/\d])/);
                if (URIs.length !== 0) {
                    chakram.setRequestDefaults({ baseUrl: URIs[0] });
                    done();
                }
            });
        });

        after(function () {
            process.kill(-dbms.pid);
            fs.closeSync(fd);
        });
    }
);