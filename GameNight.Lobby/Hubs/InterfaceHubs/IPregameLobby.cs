using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNight.Lobby.Hubs.InterfaceHubs
{
    public interface IPregameLobby
    {
        Task PlayerJoined(object player);
        Task PlayerToggleReadyUp(Guid deviceKey, bool isReady);
    }
}
