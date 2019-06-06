using System;
using System.Threading.Tasks.Dataflow;
using NUnit.Framework;

namespace ETLWorkflows.Core.Tests
{
    [TestFixture]
    public class EtlExecutionDataflowBlockOptionsTests
    {
        private EtlExecutionDataflowBlockOptions _etlExecutionDataflowBlockOptions;
        private DataflowBlockOptions _producerDataflowBlockOptions { get; set; }
        private ExecutionDataflowBlockOptions _loadDataflowBlockOptions { get; set; }
        private ExecutionDataflowBlockOptions _extractDataflowBlockOptions { get; set; }
        private ExecutionDataflowBlockOptions _transformDataflowBlockOptions { get; set; }
        private ExecutionDataflowBlockOptions _onLoadCompletedDataflowBlockOptions { get; set; }
        private ExecutionDataflowBlockOptions _onExtractCompletedDataflowBlockOptions { get; set; }
        private ExecutionDataflowBlockOptions _onTransformCompletedDataflowBlockOptions { get; set; }

        [SetUp]
        public void SetUp()
        {
            _producerDataflowBlockOptions = new DataflowBlockOptions() {BoundedCapacity = 2};
            _extractDataflowBlockOptions = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1 };
            _onExtractCompletedDataflowBlockOptions = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2 };
            _transformDataflowBlockOptions = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 3 };
            _onTransformCompletedDataflowBlockOptions = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 };
            _loadDataflowBlockOptions = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 8 };
            _onLoadCompletedDataflowBlockOptions = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 16 };

            _etlExecutionDataflowBlockOptions = new EtlExecutionDataflowBlockOptions(
                _producerDataflowBlockOptions,
                _extractDataflowBlockOptions,
                _onExtractCompletedDataflowBlockOptions,
                _transformDataflowBlockOptions,
                _onTransformCompletedDataflowBlockOptions,
                _loadDataflowBlockOptions,
                _onLoadCompletedDataflowBlockOptions);
        }

        [Test]
        public void Given_All_Parameters_When_Creating_EtlExecutionDataflowBlockOptions_Then_It_Can_Create_Instance()
        {
            Assert.IsInstanceOf<EtlExecutionDataflowBlockOptions>(_etlExecutionDataflowBlockOptions);
        }

        [Test]
        public void Given_Missing_ProducerDataflowBlockOptions_When_Creating_EtlExecutionDataflowBlockOptions_Then_It_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EtlExecutionDataflowBlockOptions(
                null,
                _extractDataflowBlockOptions,
                _onExtractCompletedDataflowBlockOptions,
                _transformDataflowBlockOptions,
                _onTransformCompletedDataflowBlockOptions,
                _loadDataflowBlockOptions,
                _onLoadCompletedDataflowBlockOptions));

            Assert.That(ex.ParamName, Is.EqualTo("producerDataflowBlockOptions"));
        }

        [Test]
        public void Given_Missing_ExtractDataflowBlockOptions_When_Creating_EtlExecutionDataflowBlockOptions_Then_It_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EtlExecutionDataflowBlockOptions(
                _producerDataflowBlockOptions,
                null,
                _onExtractCompletedDataflowBlockOptions,
                _transformDataflowBlockOptions,
                _onTransformCompletedDataflowBlockOptions,
                _loadDataflowBlockOptions,
                _onLoadCompletedDataflowBlockOptions));

            Assert.That(ex.ParamName, Is.EqualTo("extractDataflowBlockOptions"));
        }

        [Test]
        public void Given_Missing_OnExtractCompletedDataflowBlockOptions_When_Creating_EtlExecutionDataflowBlockOptions_Then_It_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EtlExecutionDataflowBlockOptions(
                _producerDataflowBlockOptions,
                _extractDataflowBlockOptions,
                null,
                _transformDataflowBlockOptions,
                _onTransformCompletedDataflowBlockOptions,
                _loadDataflowBlockOptions,
                _onLoadCompletedDataflowBlockOptions));

            Assert.That(ex.ParamName, Is.EqualTo("onExtractCompletedDataflowBlockOptions"));
        }

        [Test]
        public void Given_Missing_TransformDataflowBlockOptions_When_Creating_EtlExecutionDataflowBlockOptions_Then_It_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EtlExecutionDataflowBlockOptions(
                _producerDataflowBlockOptions,
                _extractDataflowBlockOptions,
                _onExtractCompletedDataflowBlockOptions,
                null,
                _onTransformCompletedDataflowBlockOptions,
                _loadDataflowBlockOptions,
                _onLoadCompletedDataflowBlockOptions));

            Assert.That(ex.ParamName, Is.EqualTo("transformDataflowBlockOptions"));
        }

        [Test]
        public void Given_Missing_OnTransformCompletedDataflowBlockOptions_When_Creating_EtlExecutionDataflowBlockOptions_Then_It_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EtlExecutionDataflowBlockOptions(
                _producerDataflowBlockOptions,
                _extractDataflowBlockOptions,
                _onExtractCompletedDataflowBlockOptions,
                _transformDataflowBlockOptions,
                null,
                _loadDataflowBlockOptions,
                _onLoadCompletedDataflowBlockOptions));

            Assert.That(ex.ParamName, Is.EqualTo("onTransformCompletedDataflowBlockOptions"));
        }

        [Test]
        public void Given_Missing_LoadDataflowBlockOptions_When_Creating_EtlExecutionDataflowBlockOptions_Then_It_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EtlExecutionDataflowBlockOptions(
                _producerDataflowBlockOptions,
                _extractDataflowBlockOptions,
                _onExtractCompletedDataflowBlockOptions,
                _transformDataflowBlockOptions,
                _onTransformCompletedDataflowBlockOptions,
                null,
                _onLoadCompletedDataflowBlockOptions));

            Assert.That(ex.ParamName, Is.EqualTo("loadDataflowBlockOptions"));
        }

        [Test]
        public void Given_Missing_OnLoadCompletedDataflowBlockOptions_When_Creating_EtlExecutionDataflowBlockOptions_Then_It_Throws()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new EtlExecutionDataflowBlockOptions(
                _producerDataflowBlockOptions,
                _extractDataflowBlockOptions,
                _onExtractCompletedDataflowBlockOptions,
                _transformDataflowBlockOptions,
                _onTransformCompletedDataflowBlockOptions,
                _loadDataflowBlockOptions,
                null));

            Assert.That(ex.ParamName, Is.EqualTo("onLoadCompletedDataflowBlockOptions"));
        }
    }
}
