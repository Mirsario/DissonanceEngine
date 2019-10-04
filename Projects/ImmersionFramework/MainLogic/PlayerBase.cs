using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public abstract class PlayerBase<TPlayer,TLocalPlayer,TEntity> : EntityController<TEntity>, IPlayer
		where TPlayer : PlayerBase<TPlayer,TLocalPlayer,TEntity>
		where TLocalPlayer : PlayerBase<TPlayer,TLocalPlayer,TEntity>, ILocalPlayer
		where TEntity : EntityBase
	{
		public static TPlayer[] players;
		public static TLocalPlayer[] localPlayers;

		public static int PlayerCount => players.Length;
		public static int LocalPlayerCount => localPlayers.Length;

		public abstract bool IsLocal { get; }

		public int Id { get; protected set; }

		public PlayerBase(int id)
		{
			Id = id;
		}

		public virtual void FixedUpdate()
		{
			CopyInputs();
		}
		public virtual void RenderUpdate()
		{
			
		}

		protected override void OnSwitchEntity(TEntity prevEntity,TEntity newEntity)
		{
			base.OnSwitchEntity(prevEntity,newEntity);

			prevEntity?.UpdateIsPlayer(false);
			newEntity?.UpdateIsPlayer(true);
		}

		public static TPlayer GetPlayer(int index) => index>=0 && index<PlayerCount ? players[index] : null;
		public static TLocalPlayer GetLocalPlayer(int index) => index>=0 && index<LocalPlayerCount ? localPlayers[index] : null;
		public static bool TryGetPlayer(int index,out TPlayer player) => (player = GetPlayer(index))!=null;
		public static bool TryGetLocalPlayer(int index,out TLocalPlayer player) => (player = GetLocalPlayer(index))!=null;
	}
}
