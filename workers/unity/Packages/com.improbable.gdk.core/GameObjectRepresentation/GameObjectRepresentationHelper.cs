﻿using Unity.Entities;

namespace Improbable.Gdk.Core.GameObjectRepresentation
{
    public static class GameObjectRepresentationHelper
    {
        public static void AddSystems(World world)
        {
            world.GetOrCreateManager<GameObjectDispatcherSystem>();
            world.GetOrCreateManager<EntityGameObjectLinkerSystem>();
            world.GetOrCreateManager<GameObjectWorldCommandSystem>();
        }
    }
}
