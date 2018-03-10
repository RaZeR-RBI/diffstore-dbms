var fs = require('fs');
var chakram = require('chakram');
var spawn = require('child_process').spawn;

var schemaFile = 'schema.json';

describe('Test suite',
    function () {
        var schema = null;
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
                stdio: [fd, 'pipe', 'pipe']
            });
            dbms.stdout.on('data', function (data) {
                done();
            });
            dbms.stderr.on('data', function (data) {
                throw new Error('Cannot start DBMS');
            });
        });

        after(function () {
            process.kill(-dbms.pid);
            fs.closeSync(fd);
        });


        it('should return the schema JSON on index page', function () {
            // TODO
        });
    }
);