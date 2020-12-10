using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ethereality.DoublyConnectedEdgeList.Tests
{
    public class VertexTests
    {
        [Fact]
        public void When_constructing_Vertex_Given_null_point_Should_throw_ArgumentNullException()
        {
            var action = new Action(() => new Vertex<TestSegment, TestPoint>(null));

            action.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("point");
        }
    }
}
