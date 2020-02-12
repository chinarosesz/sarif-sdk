﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.WorkItemFiling;
using Moq;
using Xunit;

namespace Microsoft.CodeAnalysis.Sarif.WorkItemFiling
{
    public class WorkItemFilerTests
    {
        private static readonly Uri s_testUri = new Uri("https://github.com/Microsoft/sarif-sdk");

        [Fact]
        public void WorkItemFiler_ValidatesSarifLogFileContentsArgument()
        {
            var filer = CreateWorkItemFiler();

            Action action = () => filer.FileWorkItems(sarifLogFileContents: null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WorkItemFiler_ValidatesSarifLogFileArgument()
        {
            var filer = CreateWorkItemFiler();

            Action action = () => filer.FileWorkItems(sarifLog: null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WorkItemFiler_ValidatesSarifLogFileLocationArgument()
        {
            var filer = CreateWorkItemFiler();

            Action action = () => filer.FileWorkItems(sarifLogFileLocation: null);

            action.Should().Throw<ArgumentNullException>();
        }

        private static SarifWorkItemFiler CreateWorkItemFiler()
            => CreateMockSarifWorkItemFiler();

        private static SarifWorkItemFiler CreateMockSarifWorkItemFiler()
        {
            var mockFilingTarget = new Mock<SarifWorkItemFiler>();

            mockFilingTarget
                .Setup(x => x.FileWorkItems(It.IsAny<string>()))
                .CallBase(); 

            mockFilingTarget
                .Setup(x => x.FileWorkItems(It.IsAny<SarifLog>()))
                .CallBase();

            mockFilingTarget
                .Setup(x => x.FileWorkItems(It.IsAny<Uri>()))
                .CallBase();

            // Moq magic: you can return whatever was passed to a method by providing
            // a lambda (rather than a fixed value) to Returns or ReturnsAsync.
            // https://stackoverflow.com/questions/996602/returning-value-that-was-passed-into-a-method
            mockFilingTarget
                .Setup(x => x.FileWorkItems(
                    It.IsAny<Uri>(), 
                    It.IsAny<IList<WorkItemModel<SarifWorkItemData>>>(),
                    It.IsAny<string>()))
                .ReturnsAsync((Uri uri, IList<WorkItemModel<SarifWorkItemData>> resultGroups, string securityToken) => resultGroups);

            return mockFilingTarget.Object;
        }

        private class TestFilingClient : FilingClient<SarifWorkItemData>
        {
            public TestFilingClient(SarifWorkItemData configuration) : base(configuration)
            {

            }

            public override Task Connect(string personalAccessToken = null)
            {
                throw new NotImplementedException();
            }

            public override Task<IEnumerable<WorkItemModel<SarifWorkItemData>>> FileWorkItems(IEnumerable<WorkItemModel<SarifWorkItemData>> workItemModels)
            {
                throw new NotImplementedException();
            }
        }
    }
}