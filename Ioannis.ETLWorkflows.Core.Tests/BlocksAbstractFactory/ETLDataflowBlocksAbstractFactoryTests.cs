using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Ioannis.ETLWorkflows.Core.BlocksAbstractFactory;
using Ioannis.ETLWorkflows.Core.Models;
using NUnit.Framework;

namespace Ioannis.ETLWorkflows.Core.Tests.BlocksAbstractFactory
{
    [TestFixture]
    public class ETLDataflowBlocksAbstractFactoryTests
    {
        private IETLDataflowBlocksAbstractFactory _dataflowBlocksAbstractFactory;

        [SetUp]
        public void SetUp()
        {
            _dataflowBlocksAbstractFactory = new ETLDataflowBlocksAbstractFactory() { EtlExecutionDataflowBlockOptions = EtlExecutionDataflowBlockOptions.DefaultOptions };
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_When_Setting_Options_For_First_Time_Then_It_Stores_The_Options()
        {
            var dataflowBlocksAbstractFactory = new ETLDataflowBlocksAbstractFactory
            {
                EtlExecutionDataflowBlockOptions = EtlExecutionDataflowBlockOptions.DefaultOptions
            };

            Assert.That(_dataflowBlocksAbstractFactory.EtlExecutionDataflowBlockOptions, Is.EqualTo(EtlExecutionDataflowBlockOptions.DefaultOptions));
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_When_Setting_Options_For_Sencond_Time_Then_It_Throws()
        {
            var dataflowBlocksAbstractFactory = new ETLDataflowBlocksAbstractFactory
            {
                EtlExecutionDataflowBlockOptions = EtlExecutionDataflowBlockOptions.DefaultOptions
            };

            var ex = Assert.Throws<InvalidOperationException>(() =>
                _dataflowBlocksAbstractFactory.EtlExecutionDataflowBlockOptions =
                    EtlExecutionDataflowBlockOptions.DefaultOptions);

            Assert.That(ex.Message.Contains("Attempt to set again"));
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_When_Creating_Producer_Then_It_Can_Create_Block()
        {
            var producer = _dataflowBlocksAbstractFactory.CreateProducerBlock<int>();

            Assert.IsInstanceOf<BufferBlock<TriggerRequest>>(producer);
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_When_Creating_Extractor_Block_Then_It_Can_Create_Block()
        {
            Task<string> Func(TriggerRequest request) => Task.FromResult(request.ToString());
            var transformer = _dataflowBlocksAbstractFactory.CreateExtractBlock<int, string>(Func);

            Assert.IsInstanceOf<TransformBlock<TriggerRequest, string>>(transformer);
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_When_Creating_Extractor_Completion_Block_Then_It_Can_Create_Block()
        {
            var func = new Func<int, Task<int>>(Task.FromResult);
            var transformer = _dataflowBlocksAbstractFactory.CreateExtractCompletedBlock<int>(func);

            Assert.IsInstanceOf<TransformBlock<int, int>>(transformer);
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_When_Creating_Transformer_Block_Then_It_Can_Create_Block()
        {
            Task<string> Func(int i) => Task.FromResult(i.ToString());
            var transformer = _dataflowBlocksAbstractFactory.CreateTransformBlock<int, string>(Func);

            Assert.IsInstanceOf<TransformBlock<int, string>>(transformer);
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_When_Creating_Transformer_Completion_Block_Then_It_Can_Create_Block()
        {
            var func = new Func<int, Task<int>>(Task.FromResult);
            var transformer = _dataflowBlocksAbstractFactory.CreateTransformCompletedBlock<int>(func);

            Assert.IsInstanceOf<TransformBlock<int, int>>(transformer);
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_When_Creating_Loader_Block_Then_It_Can_Create_Block()
        {
            Task<int> Func(int i) => Task.FromResult(i);
            var transformer = _dataflowBlocksAbstractFactory.CreateLoadBlock<int, int>(Func);

            Assert.IsInstanceOf<TransformBlock<int, int>>(transformer);
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_When_Creating_Loader_Completion_Block_Then_It_Can_Create_Block()
        {
            Task<string> Func(int i) => Task.FromResult(i.ToString());
            var transformer = _dataflowBlocksAbstractFactory.CreateLoadCompletedBlock<int>(Func);

            Assert.IsInstanceOf<ActionBlock<int>>(transformer);
        }
    }
}
