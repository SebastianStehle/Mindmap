// ==========================================================================
// VisibilityUpdaterTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Hercules.Model;
using Hercules.Model.Layouting;
using Hercules.Model.Rendering;
using Rhino.Mocks;
using Tests.Given;
using Xunit;

namespace Tests.Facts
{
    public class VisibilityUpdaterTest : GivenMocks
    {
        private readonly IRenderScene scene;
        private readonly ILayout layout;

        public VisibilityUpdaterTest()
        {
            scene = Mocks.StrictMock<IRenderScene>();

            layout = Mocks.StrictMock<ILayout>();
        }

        [Fact]
        public void VisibilityTest()
        {
            var document = new Document(Guid.NewGuid());
            var child1 = document.Root.AddChildTransactional();
            var child11 = child1.AddChildTransactional();
            var child111 = child11.AddChildTransactional();
            var child112 = child111.AddSibilingTransactional();

            child11.ToggleCollapseTransactional();

            var renderNodeChild1 = Mocks.StrictMock<IRenderNode>();
            var renderNodeChild11 = Mocks.StrictMock<IRenderNode>();
            var renderNodeChild111 = Mocks.StrictMock<IRenderNode>();
            var renderNodeChild112 = Mocks.StrictMock<IRenderNode>();

            using (Record())
            {
                scene.Expect(x => x.FindRenderNode(child1)).Return(renderNodeChild1);
                scene.Expect(x => x.FindRenderNode(child11)).Return(renderNodeChild11);
                scene.Expect(x => x.FindRenderNode(child111)).Return(renderNodeChild111);
                scene.Expect(x => x.FindRenderNode(child112)).Return(renderNodeChild112);

                renderNodeChild1.Expect(x => x.Show());
                renderNodeChild11.Expect(x => x.Show());
                renderNodeChild111.Expect(x => x.Hide());
                renderNodeChild112.Expect(x => x.Hide());
            }

            using (Playback())
            {
                new VisibilityUpdater<ILayout>(layout, scene, document).UpdateVisibility();
            }
        }
    }
}
