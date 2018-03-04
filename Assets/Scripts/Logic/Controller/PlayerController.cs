using UnityEngine;
using UnityEngine.Networking;

namespace Nexus.Logic.Controller
{
    public class PlayerController : NetworkBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (isLocalPlayer)
            {
                var x = Input.GetAxis("Horizontal") * Time.deltaTime * 50.0f;
                var z = Input.GetAxis("Vertical") * Time.deltaTime * 50.0f;

                //transform.Rotate(0, x, 0);
                transform.Translate(x, 0, z);
            }
        }

        public override void OnStartLocalPlayer()
        {
            GetComponentInChildren< SpriteRenderer>().material.color = Color.blue;
        }
    }
}