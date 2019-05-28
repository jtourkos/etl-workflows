# ETL Workflows 
[![Build Status](https://travis-ci.org/g-tourkogiorgis/ETLWorkflows.svg?branch=master)](https://travis-ci.org/g-tourkogiorgis/ETLWorkflows)  
[![NuGet](https://img.shields.io/nuget/v/ETLWorkflows.SDK.svg)](https://www.nuget.org/packages/ETLWorkflows.SDK/)

[ETL Workflows](https://github.com/g-tourkogiorgis/ETLWorkflows/wiki) is a lightweight .NET framework for building simple [ETL](https://en.wikipedia.org/wiki/Extract,_transform,_load) processes that builds upon Microsoft [TPL Dataflow](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library) library.  

## Installation
- **NuGet**: The package is available on NuGet. You can view the NuGet [package page](https://www.nuget.org/packages/ETLWorkflows.SDK/) for more details.
- **Console Package Manager**: `PM> Install-Package ETLWorkflows.SDK -Version version-number` and replace `version-number` with the version number you want.

## Quick start

1. Create a class that represents your ETL workflow.
2. Subclass your workflow to inherit from `ETLWorkflows.SDK.ETLWorkflowBase`.
3. Implement the abstract methods for your Extract, Transform and Load [steps](https://github.com/g-tourkogiorgis/ETLWorkflows/blob/b75374d40b54fed18b35f1f6a9d7fa94e3b1d10e/ETLWorkflows.SDK/ETLWorkflowBase.cs#L122).
4. Optionally, implement the [hook methods](https://github.com/g-tourkogiorgis/ETLWorkflows/blob/b75374d40b54fed18b35f1f6a9d7fa94e3b1d10e/ETLWorkflows.SDK/ETLWorkflowBase.cs#L148).
5. Implement the abstract method `FeedProducerAsync`.
6. From your client (usually the Main method of a Console application) call the `StartWorkflowAsync` method on your workflow class.

Intellisense will help you a lot, as methods are well documented.  
Please visit the [Wiki](https://github.com/g-tourkogiorgis/ETLWorkflows/wiki) (WIP) for a thorough explanation of the whole framework, if you feel like you need a more in-depth guide :)

## License
ETL Workflows is licensed under the [GNU General Public License v3.0](https://github.com/g-tourkogiorgis/ETLWorkflows/blob/master/LICENSE) license.