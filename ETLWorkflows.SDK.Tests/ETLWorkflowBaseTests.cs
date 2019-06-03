using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ETLWorkflows.Core;
using ETLWorkflows.Core.BlocksAbstractFactory;
using ETLWorkflows.SDK;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class ETLWorkflowBaseTests
    {
        private Mock<ILogger> _loggerMock;
        private ETLWorkflowTest _workflow;
        private EtlExecutionDataflowBlockOptions _testOptions;
        private Mock<IETLDataflowBlocksAbstractFactory> _etlBlocksAbstractFactoryMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _testOptions = new EtlExecutionDataflowBlockOptions(
                new DataflowBlockOptions() { BoundedCapacity = 1000 },
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 },
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 },
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 },
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 },
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 },
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 }
            );

            _etlBlocksAbstractFactoryMock = new Mock<IETLDataflowBlocksAbstractFactory>();
            _etlBlocksAbstractFactoryMock.SetupSet(x => x.EtlExecutionDataflowBlockOptions = _testOptions);

            _workflow = new ETLWorkflowTest(_loggerMock.Object, _etlBlocksAbstractFactoryMock.Object)
            {
                TestEtlExecutionDataflowBlockOptions = _testOptions
            };
        }

        [Test]
        public void Given_Logger_Dependency_When_Creating_A_Workflow_Then_It_Can_Create_Instance()
        {
            Assert.IsInstanceOf<ETLWorkflowBase<int, string, int, bool>>(_workflow);
        }

        [Test]
        public void Given_ETLDataflowBlocksAbstractFactory_Dependency_Is_Missing_When_Creating_A_Workflow_Then_It_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new ETLWorkflowTest(_loggerMock.Object, null));
            Assert.That(ex.ParamName == "etlBlocksAbstractFactory");
        }

        [Test]
        public async Task Given_A_Workflow_When_Starting_Without_Setting_Workflow_Options_Then_It_Gets_The_Default_Options()
        {
            await _workflow.StartWorkflowAsync(default);
            _etlBlocksAbstractFactoryMock.VerifySet(f => f.EtlExecutionDataflowBlockOptions = EtlExecutionDataflowBlockOptions.DefaultOptions);
        }

        [Test]
        public async Task Given_A_Workflow_When_Starting_Then_It_Creates_The_Blocks()
        {
            await _workflow.StartWorkflowAsync(default);

            _etlBlocksAbstractFactoryMock.Verify(f => f.CreateProducerBlock<int>());

            _etlBlocksAbstractFactoryMock.Verify(f => f.CreateExtractBlock<int, string>(It.IsAny<Func<TriggerRequest<int>, Task<string>>>()));
            _etlBlocksAbstractFactoryMock.Verify(f => f.CreateExtractCompletedBlock<string>(It.IsAny<Func<string, Task<string>>>()));

            _etlBlocksAbstractFactoryMock.Verify(f => f.CreateTransformBlock<string, int>(It.IsAny<Func<string, Task<int>>>()));
            _etlBlocksAbstractFactoryMock.Verify(f => f.CreateTransformCompletedBlock<int>(It.IsAny<Func<int, Task<int>>>()));

            _etlBlocksAbstractFactoryMock.Verify(f => f.CreateLoadBlock<int, bool>(It.IsAny<Func<int, Task<bool>>>()));
            _etlBlocksAbstractFactoryMock.Verify(f => f.CreateLoadCompletedBlock<bool>(It.IsAny<Func<bool, Task>>()));
        }

        [Test]
        public void Given_A_Workflow_When_Starting_Then_It_Runs_ETL_As_Expected()
        {
            var factory = new ETLDataflowBlocksAbstractFactory();
            var workflow = new ETLWorkflowTest(_loggerMock.Object, factory) { TestEtlExecutionDataflowBlockOptions = _testOptions };
            workflow.StartWorkflowAsync(default).Wait();

            _loggerMock.Verify(x => x.Info(It.Is<string>(s => s.Contains("ExtractAsync called"))), Times.Once);
            _loggerMock.Verify(x => x.Info(It.Is<string>(s => s.Contains("TriggerRequest payload: 1"))), Times.Once);
            _loggerMock.Verify(x => x.Info(It.Is<string>(s => s.Contains("Extract step completed"))), Times.Once);
            _loggerMock.Verify(x => x.Info(It.Is<string>(s => s.Contains("TransformAsync called"))), Times.Once);
            _loggerMock.Verify(x => x.Info(It.Is<string>(s => s.Contains("Transformed to 1"))), Times.Once);
            _loggerMock.Verify(x => x.Info(It.Is<string>(s => s.Contains("Transform step completed"))), Times.Once);
            _loggerMock.Verify(x => x.Info(It.Is<string>(s => s.Contains("LoadAsync called"))), Times.Once);
            _loggerMock.Verify(x => x.Info(It.Is<string>(s => s.Contains("Loaded result True"))), Times.Once);
            _loggerMock.Verify(x => x.Info(It.Is<string>(s => s.Contains("Cleaning up"))), Times.Once);
        }
    }

    public class ETLWorkflowTest : ETLWorkflowBase<int, string, int, bool>
    {
        public EtlExecutionDataflowBlockOptions TestEtlExecutionDataflowBlockOptions { get; set; }

        public ETLWorkflowTest(ILogger logger) : base(logger)
        {
        }

        public ETLWorkflowTest(ILogger logger, IETLDataflowBlocksAbstractFactory etlBlocksAbstractFactory) : base(logger, etlBlocksAbstractFactory)
        {
            _logger = logger;
        }

        public override async Task FeedProducerAsync(ITargetBlock<TriggerRequest<int>> targetBlock,
            CancellationToken cancellationToken, ILogger logger = null)
        {
            await targetBlock.SendAsync(new TriggerRequest<int>() { Payload = 1 });
            targetBlock.Complete();
        }

        protected override Task<string> ExtractAsync(TriggerRequest<int> request)
        {
            _logger.Info("ExtractAsync called");
            _logger.Info($"TriggerRequest payload: {request.Payload}");
            return Task.FromResult(request.Payload.ToString());
        }


        protected override Task<int> TransformAsync(string extractorResult)
        {
            _logger.Info("TransformAsync called");
            _logger.Info($"Transformed to {int.Parse(extractorResult)}");
            return Task.FromResult(int.Parse(extractorResult));
        }

        protected override Task<bool> LoadAsync(int transformerResult)
        {
            _logger.Info("LoadAsync called");
            _logger.Info($"Loaded result {transformerResult == 1}");
            return Task.FromResult(transformerResult == 1);
        }

        //protected override EtlExecutionDataflowBlockOptions GetWorkflowBlockOptions()
        //{
        //    return TestEtlExecutionDataflowBlockOptions;
        //}
    }
}