using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mono
{
    public class Character : MonoBehaviour
    {

        public float speed = 10;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            float translation = Input.GetAxis("Vertical") * speed;
            float straffing = Input.GetAxis("Horizontal") * speed;
            translation *= Time.deltaTime;
            straffing *= Time.deltaTime;

            transform.Translate(straffing, 0, translation);

            if (Input.GetKeyDown("escape"))
                Cursor.lockState = CursorLockMode.None;
        }
    }
}