using System.Threading.Tasks;
using NUnit.Framework;
using ReactIb.Utils;

namespace ReactIb.UnitTests
{
    [TestFixture]
    public class AsyncManualResetEventTests
    {
        [Test]
        public void TestAwaitUnsetHandle()
        {
            var waitHandle = new AsyncManualResetEvent();

            var task = waitHandle.WaitAsync();
            task.Wait(200);

            Assert.IsFalse(task.IsCompleted);
        }

        [Test]
        public void TestAwaitSetHandle()
        {
            var waitHandle = new AsyncManualResetEvent();
            waitHandle.Set();

            var task = waitHandle.WaitAsync();
            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public async void TestAwaitHandleUntilSet()
        {
            var waitHandle = new AsyncManualResetEvent();
            var done = false;

            var task = waitHandle.WaitAsync().ContinueWith(_ => done = true);

            await Task.Delay(200);
            Assert.IsFalse(done);

            waitHandle.Set();

            await task;
            Assert.IsTrue(done);
        }

        [Test]
        public async void TestResetCausesWait()
        {
            var waitHandle = new AsyncManualResetEvent();
            var done = false;

            waitHandle.Set();
            waitHandle.Reset();

            var task = waitHandle.WaitAsync().ContinueWith(_ => done = true);

            await Task.Delay(200);
            Assert.IsFalse(done);

            waitHandle.Set();

            await task;
            Assert.IsTrue(done);
        }

        [Test]
        public async void TestSetMultipleTimes()
        {
            var waitHandle = new AsyncManualResetEvent();
            var done = false;

            waitHandle.Set();
            waitHandle.Set();
            waitHandle.Set();

            var task = waitHandle.WaitAsync().ContinueWith(_ => done = true);

            await task;
            Assert.IsTrue(done);
        }
    }
}
