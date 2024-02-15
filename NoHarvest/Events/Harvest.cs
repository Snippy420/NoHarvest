using Microsoft.Extensions.Configuration;
using OpenMod.API.Eventing;
using OpenMod.API.Persistence;
using OpenMod.Core.Users;
using OpenMod.Unturned.Building.Events;
using OpenMod.Unturned.Users;
using OpenMod.Unturned.Users.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoHarvest.Events
{
    public class Harvest : IEventListener<UnturnedPlantHarvestingEvent>
    {
        private readonly UnturnedUserDirectory m_UnturnedUserDirectory;
        private readonly IConfiguration m_Configuration;
        private readonly IDataStore m_DataStore;

        public Harvest(UnturnedUserDirectory unturnedUserDirectory, IConfiguration configuration)
        {
            m_UnturnedUserDirectory = unturnedUserDirectory;
            m_Configuration = configuration;
        }

        public async Task HandleEventAsync(object sender, UnturnedPlantHarvestingEvent @event)
        {
            var owner = @event.Buildable.BarricadeData.owner.ToString();
            var player = @event.InstigatorSteamId.ToString();

            if (owner == player)
            {
                return;
            }
            else
            {
                @event.IsCancelled = true;
                @event.Instigator.PrintMessageAsync("You can not harvest other players crops.");
            }
        }
    }
}
