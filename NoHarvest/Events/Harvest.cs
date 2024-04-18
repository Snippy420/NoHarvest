using Microsoft.Extensions.Configuration;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.API.Users;
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
        private readonly IConfiguration _Configuration;
        private readonly bool _enable_no_harvest;
        private readonly bool _allow_admin_bypass;
        private readonly bool _allow_group_harvest;

        public Harvest(IConfiguration configuration)
        {
            _Configuration = configuration;

            _enable_no_harvest = _Configuration.GetValue<bool>("enable_no_harvest");
            _allow_admin_bypass = _Configuration.GetValue<bool>("allow_admin_bypass");
            _allow_group_harvest = _Configuration.GetValue<bool>("allow_group_harvest");
        }

        public async Task HandleEventAsync(object sender, UnturnedPlantHarvestingEvent @event)
        {
            if (!_enable_no_harvest)
            {
                return;
            }

            var owner = @event.Buildable.BarricadeData.owner;
            var player = @event.Instigator;
            if (owner.ToString() == player.SteamId.ToString() || player.Player.channel.owner.isAdmin && _allow_admin_bypass || await @event.Buildable.Ownership.HasAccessAsync(player) && _allow_group_harvest)
            {
                return;
            }

            @event.IsCancelled = true;
            player.PrintMessageAsync("You can not harvest other players crops.");
        }
    }
}
