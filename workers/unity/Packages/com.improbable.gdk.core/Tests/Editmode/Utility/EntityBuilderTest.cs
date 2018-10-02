using System;
using Improbable.Gdk.Core.Utility;
using NUnit.Framework;
using Improbable.Worker.Core;

namespace Improbable.Gdk.Core.Tests.EditmodeTests.Utility
{
    [TestFixture]
    public class EntityBuilderTest
    {
        [Test]
        public void EntityBuilder_should_throw_exception_if_no_position_component_is_added()
        {
            var builder = EntityBuilder.Begin();
            Assert.Throws<InvalidEntityException>(() => builder.Build());
        }

        [Test]
        public void EntityBuilder_should_throw_exception_if_component_is_added_twice()
        {
            var builder = EntityBuilder.Begin();
            builder.AddPosition(0, 0, 0, "write-access");
            Assert.Throws<InvalidOperationException>(() => builder.AddPosition(0, 0, 0, "write-access"));

            DisposeEntity(builder.Build());
        }

        [Test]
        public void EntityBuilder_should_throw_exception_if_Build_called_more_than_once()
        {
            var builder = EntityBuilder.Begin();
            builder.AddPosition(0, 0, 0, "write-access");
            var entity = builder.Build();
            Assert.Throws<InvalidOperationException>(() => builder.Build());

            DisposeEntity(entity);
        }

        [Test]
        public void EntityBuilder_should_Build_if_only_position_is_added()
        {
            var builder = EntityBuilder.Begin();
            builder.AddPosition(0, 0, 0, "write-access");

            Entity entity = new Entity(); // Stops IDE complaining about null check below.

            // Wrap in try - finally to ensure dispose gets called even if the assert fails.
            try
            {
                Assert.DoesNotThrow(() => entity = builder.Build());
            }
            finally
            {
                DisposeEntity(entity);
            }
        }

        [Test]
        public void EntityBuilder_should_Build_if_arbitrary_components_are_added()
        {
            var builder = EntityBuilder.Begin();
            builder.AddPosition(0, 0, 0, "write-access");
            builder.AddComponent(GetComponentDataWithId(1000), "write-access");
            builder.AddComponent(GetComponentDataWithId(1001), "write-access");
            var entity = new Entity(); // Stops IDE complaining about null check below.

            // Wrap in try - finally to ensure dispose gets called even if the assert fails.
            try
            {
                Assert.DoesNotThrow(() => entity = builder.Build());
            }
            finally
            {
                DisposeEntity(entity);
            }
        }

        private ComponentData GetComponentDataWithId(uint componentId)
        {
            var schemaComponentData = new SchemaComponentData(componentId);
            return new ComponentData(schemaComponentData);
        }

        /// <summary>
        ///     Disposes the underlying SchemaComponentData in native memory for a given Entity.
        ///     If the memory is not manually disposed, it will cause a leak!
        /// </summary>
        private void DisposeEntity(Entity entity)
        {
            var componentIds = entity.GetComponentIds();

            foreach (var id in componentIds)
            {
                var componentData = entity.Get(id);
                componentData.Value.SchemaData.Value.Dispose();
            }
        }
    }
}
