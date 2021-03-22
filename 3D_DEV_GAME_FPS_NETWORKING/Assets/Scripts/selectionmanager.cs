using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class selectionmanager : MonoBehaviour, IPunObservable
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    public Text score;
    private Transform _selection;
    private int scoreStart = 0;
    public float distance;
    private string standardText = "/9 Exams Collected";

    // Start is called before the first frame update
    void Start()
    {
        score.text = scoreStart.ToString() + standardText;
    }

    // Update is called once per frame
    void Update()
    {
        if (_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }
        try
        {
            GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
            foreach (var item in cameras)
            {
                Camera tempCam = item.GetComponent<Camera>();
                var ray = tempCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Examen" && hit.distance < distance)
                {
                    Debug.Log("hit");
                    var selection = hit.transform;
                    hit.transform.gameObject.GetPhotonView().RequestOwnership();
                    score.gameObject.GetPhotonView().RequestOwnership();
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    if (selectionRenderer != null)
                    {
                        selectionRenderer.material = highlightMaterial;
                        if (Input.GetMouseButtonDown(0))
                        {
                            PhotonNetwork.Destroy(hit.transform.gameObject);

                            score.gameObject.GetPhotonView().RPC("updateScore", RpcTarget.All, standardText);
                            //score.text = scoreStart.ToString() + standardText;
                        }
                    }
                    _selection = selection;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }


    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.score.text);
        }
        else
        {
            // Network player, receive data
            this.score.text = (string)stream.ReceiveNext();
        }
    }
}
