using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public override int GetHashCode()
        {
            unchecked
            {
                return (Foo * 397) ^ (Bar != null ? Bar.GetHashCode() : 0);
            }
        }
    }

    public class RemoteDBMSTest : IClassFixture<RemoteDBMSFixture>
    {
        private IDiffstoreDBMS<long, SampleSchema> db;

        public RemoteDBMSTest(RemoteDBMSFixture fixture) =>
            db = fixture.Driver;

        [Fact]
        public async void ShouldSaveEntitiesAndMakeSnapshots()
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
            var workingDir = Path.Combine(
                Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                "Standalone"
            );

            var processStart = new ProcessStartInfo("dotnet", string.Join(" ",
                "run",
                "--no-build",
                "--",
                "--store",
                "InMemory"
            ))
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = workingDir
            };

            var process = Process.Start(processStart);

            // Wait for it to start
            Console.WriteLine(process.StandardOutput.ReadLine());
            return process;
        }

        public void Dispose()
        {
            Driver.Dispose();
            backend.Kill();
        }
    }
}