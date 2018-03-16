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
                    expect(response).to.have.header('content-type', 'application/json');
                    expect(response).to.comprise.of.json(schema);
                });
        });

        it('should properly save entities', function () {
            var key = 1;
            var value = {
                foo: 1337,
                bar: 'hello'
            };
            var saveRequest = {
                makeSnapshot: true,
                value: value
            };
            return chakram.post('/entities/' + key, saveRequest)
                .then(function (response) {
                    expect(response).to.have.status(200);
                    return chakram.get('/entities/' + key);
                })
                .then(function (response) {
                    expect(response).to.have.status(200);
                    expect(response).to.have.header('content-type', 'application/json');
                    expect(response).to.comprise.of.json(value);
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