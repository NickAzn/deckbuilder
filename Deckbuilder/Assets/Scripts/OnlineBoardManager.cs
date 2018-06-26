using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class OnlineBoardManager : NetworkBoardManagerBehavior {

    public GameManager gm;
    public Player player;

    protected override void NetworkStart() {
        base.NetworkStart();
        //If not hosting, then go second
        if (!networkObject.IsServer) {
            gm.playerTurn = false;
            gm.endTurnButton.SetActive(false);
            player.IncreaseMaxMana(1);
        }
    }

    //Send RPC to end turn
    public void NetworkEndTurn() {
        networkObject.SendRpc(RPC_END_TURN, Receivers.AllBuffered, networkObject.IsServer);
    }

    //Send RPC to summon unit at spot
    public void NetworkSummonUnit(int spotIndex, string cardName) {
        networkObject.SendRpc(RPC_SUMMON_UNIT, Receivers.AllBuffered, networkObject.IsServer, spotIndex, cardName);
    }

    //Send RPC to cast spell at spot
    public void NetworkCastSpell(bool playerSide, int spotIndex, string cardName) {
        bool hostCasted = networkObject.IsServer;
        bool hostSide = true;
        if ((hostCasted && !playerSide) || (!hostCasted && playerSide)) {
            hostSide = false;
        }
        networkObject.SendRpc(RPC_CAST_SPELL, Receivers.AllBuffered, hostSide, spotIndex, cardName, hostCasted);
    }

    //RPC to summon unit at spot
    public override void SummonUnit(RpcArgs args) {
        bool hostSide = args.GetNext<bool>();
        int spotIndex = args.GetNext<int>();
        string cardName = args.GetNext<string>();

        if (networkObject.IsServer && hostSide) {
            return;
        } else if (!networkObject.IsServer && !hostSide) {
            return;
        } else {
            Card playCard = (Resources.Load("Cards/" + cardName) as GameObject).GetComponent<Card>();
            gm.enemySpots[spotIndex].AddUnit(playCard);
        }
    }

    //RPC to cast spell at spot
    public override void CastSpell(RpcArgs args) {
        bool hostSide = args.GetNext<bool>();
        int spotIndex = args.GetNext<int>();
        string cardName = args.GetNext<string>();
        bool hostCasted = args.GetNext<bool>();

        if (networkObject.IsServer && hostCasted) {
            return;
        } else if (!networkObject.IsServer && !hostCasted) {
            return;
        } else {
            Card playCard = (Resources.Load("Cards/" + cardName) as GameObject).GetComponent<Card>();
            if (playCard.crystalPact > 0) {
                gm.enemyCrystals[gm.enemySpots[spotIndex].row - 1].TakeDamage(playCard.crystalPact);
                gm.CheckGameEnded();
            }
            if ((networkObject.IsServer && hostSide) || (!networkObject.IsServer && !hostSide))
                gm.playerSpots[spotIndex].UseSpell(playCard, false);
            else
                gm.enemySpots[spotIndex].UseSpell(playCard, false);
        }
    }

    //RPC to end turn
    public override void EndTurn(RpcArgs args) {
        bool hostEnded = args.GetNext<bool>();
        if ((!networkObject.IsServer && !hostEnded) || (networkObject.IsServer && hostEnded))
            return;
        gm.EndTurn();
    }

    //Host closes the server when the match ends
    public void CloseServer() {
        if(networkObject.IsServer) {
            UDPServer server = (UDPServer)NetworkManager.Instance.Networker;
            server.Disconnect(false);
            Debug.Log("Closed Server");
        }
    }
}
