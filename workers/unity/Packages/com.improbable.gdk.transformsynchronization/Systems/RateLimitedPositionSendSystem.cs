using Improbable.Gdk.Core;
using Improbable.Transform;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Improbable.Gdk.TransformSynchronization
{
    [DisableAutoCreation]
    [UpdateAfter(typeof(DefaultUpdateLatestTransformSystem))]
    [UpdateAfter(typeof(RateLimitedTransformSendSystem))]
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class RateLimitedPositionSendSystem : ComponentSystem
    {
        private struct Data
        {
            public readonly int Length;
            public ComponentDataArray<Position.Component> PositionComponent;
            public ComponentDataArray<LastPositionSentData> LastPositionSent;
            [ReadOnly] public ComponentDataArray<TransformInternal.Component> TrasnformComponent;
            [ReadOnly] public SharedComponentDataArray<RateLimitedSendConfig> RateLimitedConfig;

            [ReadOnly] public ComponentDataArray<Authoritative<Position.Component>> DenotesAuthoritative;
        }

        [Inject] private Data data;

        protected override void OnUpdate()
        {
            for (int i = 0; i < data.Length; ++i)
            {
                var position = data.PositionComponent[i];

                var lastPositionSent = data.LastPositionSent[i];
                lastPositionSent.TimeSinceLastUpdate += Time.deltaTime;
                data.LastPositionSent[i] = lastPositionSent;

                if (lastPositionSent.TimeSinceLastUpdate <
                    1.0f / data.RateLimitedConfig[i].MaxPositionUpdateRateHz)
                {
                    continue;
                }

                var transform = data.TrasnformComponent[i];
                var coords = new Coordinates
                {
                    X = transform.Location.X,
                    Y = transform.Location.Y,
                    Z = transform.Location.Z,
                };

                if (!TransformUtils.HasMoved(coords, position.Coords))
                {
                    continue;
                }

                position.Coords = coords;
                data.PositionComponent[i] = position;

                lastPositionSent.TimeSinceLastUpdate = 0.0f;
                data.LastPositionSent[i] = lastPositionSent;
            }
        }
    }
}
