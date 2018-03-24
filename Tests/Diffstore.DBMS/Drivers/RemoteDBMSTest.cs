using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Diffstore.DBMS;
using Diffstore.Entities;
using Xunit;

namespace Tests.Diffstore.DBMS.Drivers
{
    public class SampleSchema
    {
        public int Foo { get; set; }
        public string Bar { get; set; }

        public override bool Equals(object obj) =>
            Equals(obj as SampleSchema);

        private bool Equals(SampleSchema other) =>
            (other != null) && (Foo == other.Foo) && (Bar == other.Bar);

        public override int GetHashCode() =>
            unchecked((Foo * 397) ^ (Bar != null ? Bar.GetHashCode() : 0));

        public override string ToString() => $"<Foo: {Foo}, Bar: {Bar}>";
    }



    public class RemoteDBMSTest : IClassFixture<RemoteDBMSFixture>
    {
        private IDiffstoreDBMS<long, SampleSchema> db;
        public RemoteDBMSTest(RemoteDBMSFixture fixture) => db = fixture.Driver;

        [Fact]
        public async Task ShouldSaveEntitiesAndMakeSnapshotsAsync()
        {
            var key = 1L;
            var value = new SampleSchema
            {
                Foo = 1337,
                Bar = "hello"
            };
            var expected = Entity.Create(key, value);

            await db.Save(expected);
            var snapshots = await db.GetSnapshots(key);

            Assert.Equal(expected, await db.Get(key));
            Assert.Single(snapshots);
            Assert.Equal(expected, snapshots.First().State);
        }

        [Fact]
        public async Task ShouldCheckForExistenceAndAllowDeletionAsync()
        {
            var key = 2L;
            var value = new SampleSchema
            {
                Foo = 0,
                Bar = "delete me"
            };

            await db.Save(key, value);
            Assert.True(await db.Exists(key));
            await db.Delete(key);
            Assert.False(await db.Exists(key));
        }
    }



    public class RemoteDBMSFixture : IDisposable
    {
        public IDiffstoreDBMS<long, SampleSchema> Driver { get; set; }
        private Process backend;

        public RemoteDBMSFixture()
        {
            backend = StartDBMS();
            Driver = DiffstoreDBMS.Remote<long, SampleSchema>(
                new Uri("http://localhost:8008/")
            );
        }

        private Process StartDBMS()
        {
            var workingDir = LocateDirectory(
                Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                "E2E"
            );

            // Note: there was an attempt to remove this hardcoded path
            // using 'dotnet run' - but it's child 'dotnet exec' process
            // cannot be killed without platform-specific hacks
            var dll = Path.Combine(
                Directory.GetParent(workingDir).FullName,
                "Standalone",
                "bin",
                "Debug",
                "netcoreapp2.0",
                "diffstore-dbms.dll"
            );

            var processStart = new ProcessStartInfo("dotnet", string.Join(" ",
                $"\"{dll}\"",
                "--store",
                "InMemory"
            ))
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDir
            };

            var process = Process.Start(processStart);

            // Wait for it to start
            process.StandardOutput.ReadLine();
            return process;
        }

        private string LocateDirectory(string basePath, string dirName)
        {
            var result = Directory.GetDirectories(basePath)
                .Select(Path.GetFileName)
                .Any(d => d == dirName);

            if (!result)
                return LocateDirectory(
                    Directory.GetParent(basePath).FullName,
                    dirName);

            return Path.Combine(basePath, dirName);
        }

        public void Dispose()
        {
            Driver.Dispose();
            backend.Kill();

        }
    }
}