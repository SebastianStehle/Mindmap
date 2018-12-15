// ==========================================================================
// VisibilityUpdaterTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using FakeItEasy;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Rendering;
using Xunit;

namespace Tests.Facts
{
    public class VisibilityUpdaterTest
    {
        private readonly IRenderScene scene = A.Fake<IRenderScene>();
        private readonly ILayout layout = A.Fake<ILayout>();

        [Fact]
        public void VisibilityTest()
        {
            var document = new Document(Guid.NewGuid());
            var child1 = document.Root.AddChildTransactional();
            var child11 = child1.AddChildTransactional();
            var child111 = child11.AddChildTransactional();
            var child112 = child111.AddSibilingTransactional();

            child11.ToggleCollapseTransactional();

            var renderNodeChild1 = A.Fake<IRenderNode>();
            var renderNodeChild11 = A.Fake<IRenderNode>();
            var renderNodeChild111 = A.Fake<IRenderNode>();
            var renderNodeChild112 = A.Fake<IRenderNode>();

            A.CallTo(() => scene.FindRenderNode(child1))
                .Returns(renderNodeChild1);
            A.CallTo(() => scene.FindRenderNode(child11))
                .Returns(renderNodeChild11);
            A.CallTo(() => scene.FindRenderNode(child111))
                .Returns(renderNodeChild111);
            A.CallTo(() => scene.FindRenderNode(child112))
                .Returns(renderNodeChild112);

            new VisibilityUpdater<ILayout>(layout, scene, document).UpdateVisibility();

            A.CallTo(() => renderNodeChild1.Show())
                .MustHaveHappened();
            A.CallTo(() => renderNodeChild11.Show())
                .MustHaveHappened();
            A.CallTo(() => renderNodeChild111.Hide())
                .MustHaveHappened();
            A.CallTo(() => renderNodeChild112.Hide())
                .MustHaveHappened();
        }
    }
}
