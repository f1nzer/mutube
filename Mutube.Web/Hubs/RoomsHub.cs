using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;

namespace Mutube.Web.Hubs
{
    public class RoomsHub : Hub
    {
        private static readonly ConcurrentDictionary<string, ImmutableHashSet<string>> UsersInGroups
            = new ConcurrentDictionary<string, ImmutableHashSet<string>>();
        
        [PublicAPI]
        public async Task Join(Guid roomId)
        {
            var groupId = GetGroupIdFromRoomId(roomId);

            if (UsersInGroups.TryGetValue(Context.ConnectionId, out var groups) && 
                UsersInGroups.TryUpdate(Context.ConnectionId, groups.Add(groupId), groups))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            }
            else
            {
                throw new Exception($"Can't add connection {Context.ConnectionId} to room {roomId}");
            }
        }

        public async Task Leave(Guid roomId)
        {
            var groupId = GetGroupIdFromRoomId(roomId);
            
            if (UsersInGroups.TryGetValue(Context.ConnectionId, out var groups) && 
                UsersInGroups.TryUpdate(Context.ConnectionId, groups.Remove(groupId), groups))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
            }
            else
            {
                throw new Exception($"Can't remove connection {Context.ConnectionId} from room {roomId}");
            }
        }

        public override Task OnConnectedAsync()
        {
            UsersInGroups.TryAdd(Context.ConnectionId, ImmutableHashSet.Create<string>());
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            UsersInGroups.TryRemove(Context.ConnectionId, out _);
            return base.OnDisconnectedAsync(exception);
        }

        private static string GetGroupIdFromRoomId(Guid roomId)
        {
            return $"room_{roomId}";
        }
    }
}