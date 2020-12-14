using FluentAssertions;
using System;
using Xunit;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    public class VertexTests
    {
        [Fact]
        public void When_constructing_Vertex_Given_null_point_Should_throw_ArgumentNullException()
        {
            var action = new Action(() => _ = new Vertex<TestSegment, TestPoint>(null));

            action.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("point");
        }
    }
}
