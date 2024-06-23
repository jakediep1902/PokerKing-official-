using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStream : MonoBehaviour, IPunObservable
{
    PhotonView PvCard;
    public int value = 0;
    public int ID = 0;
    private void OnEnable()
    {
        //GetComponent<AudioSource>().Play();
        //gameObject.AddComponent<PhotonView>();
        PvCard = GetComponent<PhotonView>();
        PvCard.observableSearch = PhotonView.ObservableSearch.AutoFindAll;

    }
    private void Start()
    {
        //Debug.Log("hello from Card");
        //value = int.Parse(this.gameObject.name);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(value);
            // stream.SendNext(indexBigBlind);
        }
        else if (stream.IsReading)
        {
            value = (int)stream.ReceiveNext();
            // indexBigBlind = (int)stream.ReceiveNext();
        }
    }//using
}
