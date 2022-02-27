using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MultiplayerBoard : Board
{
    // although this will contain RPC method definitions,
    // the RPC methods are be called from the PhotonView class
    // so we have created a PhotonView ref
    private PhotonView photonView;

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
    }


    public override void OnSelectedPieceMoved(Vector2 coords)
    {
        photonView.RPC(nameof(RPC_OnSelectedPieceMoved), RpcTarget.AllBuffered, new object[] { coords });
    }

    [PunRPC]
    private void RPC_OnSelectedPieceMoved(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        MoveSelectedPiece(intCoords);
    }

    public override void SetSelectedPiece(Vector2 coords)
    {
        photonView.RPC(nameof(RPC_OnSetSelectedPiece), RpcTarget.AllBuffered, new object[] { coords });

    }

    [PunRPC]
    private void RPC_OnSetSelectedPiece(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSetSelectedPiece(intCoords);
    }





}
