using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.MapObjects
{

    public interface IAssignable
    {
        int Owner { get; set; }
    }

    public interface IConsumable
    {
        Treasure Treasure { get; set; }
    }

    public interface IBeatable
    {
        Army Army { get; set; }
    }
    public class Dwelling : IAssignable
    {
        public int Owner { get; set; }
    }

    public class Mine : IAssignable, IBeatable, IConsumable
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Creeps : IBeatable, IConsumable
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Wolves : IBeatable
    {
        public Army Army { get; set; }
    }

    public class ResourcePile : IConsumable
    {
        public Treasure Treasure { get; set; }
    }

    public static class Interaction
    {
        public static void Make(Player player, object mapObject)
        {
            if (mapObject is IBeatable beatObj)
            {
                if (!player.CanBeat(beatObj.Army))
                {
                    player.Die();
                    return;
                }
            }

            if (mapObject is IConsumable consumeObj)
            {
                player.Consume(consumeObj.Treasure);
            }

            if (mapObject is IAssignable assignObj)
            {
                assignObj.Owner = player.Id;
            } 
        }
    }
}
