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
            Document document = new Document(Guid.NewGuid());
            NodeBase child1 = document.Root.AddChildTransactional();
            NodeBase child11 = child1.AddChildTransactional();
            NodeBase child111 = child11.AddChildTransactional();
            NodeBase child112 = child111.AddSibilingTransactional();

            child11.ToggleCollapseTransactional();

            IRenderNode renderNodeChild1 = Mocks.StrictMock<IRenderNode>();
            IRenderNode renderNodeChild11 = Mocks.StrictMock<IRenderNode>();
            IRenderNode renderNodeChild111 = Mocks.StrictMock<IRenderNode>();
            IRenderNode renderNodeChild112 = Mocks.StrictMock<IRenderNode>();

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
