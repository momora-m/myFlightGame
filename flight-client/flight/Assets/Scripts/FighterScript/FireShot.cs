using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class FireShot : MonoBehaviour
{
    public GameObject bullet;

    public GameObject laser;

    public Transform weapon;

    public Transform weapon2;

    private GameObject shot1;

    private GameObject shot2;

    private float shotInterval;

    private
    // Update is called once per frame
    void Update()
    {
        shotInterval += 1;

        if (shotInterval % 5 == 0)
        {

            bool Fire = CrossPlatformInputManager.GetButton("Fire1");//TODO FighterInputPS4でリファクタを行う。  
            if (Fire)
            {
                shotWeapon();
            }

            if (shot1)
            {
                deleteBullet();
            }
            //Debug.Log(Fire);
        }
        bool fireLaser = CrossPlatformInputManager.GetButton("Fire2");
        if (fireLaser)
        {
            //shotLaser();
        }
    }

    private void shotLaser()
    {
        shot2 = (GameObject)Instantiate(laser, weapon.position, Quaternion.Euler(weapon.parent.eulerAngles.x, weapon.parent.eulerAngles.y, weapon.parent.eulerAngles.z)); 
    }


    private void shotWeapon()
    {
        if (Camera.main == null)
        {
            Debug.LogError("(FlightControls) Main camera is null! Make sure the flight camera has the tag of MainCamera!");
            return;
        }

        /*shot1 = (GameObject)Instantiate(bullet, weapon.position, Quaternion.identity);

         Ray vRay;

         vRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));


         RaycastHit hit;

         //If we make contact with something in the world, we'll make the shot actually go to that point.
         if (Physics.Raycast(vRay, out hit))
         {
             shot1.transform.LookAt(hit.point);
             shot1.GetComponent<Rigidbody>().AddForce((shot1.transform.forward) * 9000f);

             //Otherwise, since the ray didn't hit anything, we're just going to guess and shoot the projectile in the general direction.
         }
         else
         {
             shot1.GetComponent<Rigidbody>().AddForce((vRay.direction) * 9000f);
         }*/
        shot1 = (GameObject)Instantiate(bullet, weapon.position, Quaternion.Euler(weapon.parent.eulerAngles.x, weapon.parent.eulerAngles.y, weapon.parent.eulerAngles.z));
        shot1.GetComponent<Rigidbody>().AddForce((shot1.transform.forward) * 9000f);
    }

    private void deleteBullet()
    {
        Destroy(shot1, 3.0f);
    }
}
