using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Card : MonoBehaviour
{
    PhotonView PvCard;
    public int value = 0;
    public int ID = 0;
    private void OnEnable()
    {
        //gameObject.AddComponent<PhotonView>();
        PvCard = GetComponent<PhotonView>();
        PvCard.observableSearch = PhotonView.ObservableSearch.AutoFindAll;
        
    }
    private void Start()
    {
        //Debug.Log("hello from Card");
        value = int.Parse(this.gameObject.name);       
    }
}
