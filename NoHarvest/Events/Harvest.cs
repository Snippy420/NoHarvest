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
        private readonly UnturnedUserDirectory m_UnturnedUserDirectory;
        private readonly IConfiguration m_Configuration;
        private readonly IDataStore m_DataStore;
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly IUserManager m_UserManager;
        private readonly bool m_enable_no_harvest;
        private readonly bool m_allow_admin_bypass;

        public Harvest(UnturnedUserDirectory unturnedUserDirectory, IConfiguration configuration, IPermissionChecker permissionChecker, IUserManager userManager)
        {
            m_UnturnedUserDirectory = unturnedUserDirectory;
            m_Configuration = configuration;
            m_PermissionChecker = permissionChecker;
            m_UserManager = userManager;

            m_enable_no_harvest = m_Configuration.GetValue<bool>("enable_no_harvest");
            m_allow_admin_bypass = m_Configuration.GetValue<bool>("allow_admin_bypass");
        }

        public async Task HandleEventAsync(object sender, UnturnedPlantHarvestingEvent @event)
        {
            if (m_enable_no_harvest)
            {
                var owner = @event.Buildable.BarricadeData.owner.ToString();
                var player = @event.InstigatorSteamId.ToString();
                var isadmin = @event.Instigator.Player.channel.owner.isAdmin;

                if (owner == player)
                {
                    return;
                }
                else
                {
                    if (isadmin && m_allow_admin_bypass)
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
    }
}
