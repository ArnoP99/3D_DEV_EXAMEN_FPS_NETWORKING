using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class SelectionManager: MonoBehaviour, IPunObservable
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    [SerializeField] private Text score;

    private Transform selection;

    private int scoreStart = 0;
    private float distance = 2.5f;
    private string standardText = "/9 Exams Collected";

    void Start()
    {
        score.text = scoreStart.ToString() + standardText;
    }

    // Update is called once per frame
    void Update()
    {
        if (selection != null)
        {
            var selectionRenderer = selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            selection = null;
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
                        }
                    }
                    this.selection = selection;
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
