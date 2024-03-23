using System;
using CsQuery;
using FluentAssertions;
using Metrics.Endpoints;
using Xunit;

namespace Metrics.Tests.Endpoints
{
    public class FlotVisualizationTests
    {
        [Fact]
        public void FlotVisualization_CanReadAppFromResource()
        {
            var html = FlotWebApp.GetFlotApp();
            html.Should().NotBeEmpty();

            Action createHtml = () => CQ.CreateDocument(html);
            createHtml.Should().NotThrow();
        }
    }
}
