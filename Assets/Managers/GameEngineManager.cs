using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameEngineManager : NetworkBehaviour
{
    List<NetworkObject> playerObjects = new();
    List<Rigidbody> playerRigidbodies = new();

    [SerializeField] Rigidbody testRB;
    [SerializeField] Transform bulletPointOne;
    [SerializeField] Transform bulletPointTwo;

    [SerializeField] GameObject bullet;

    bool canShoot = true;
    bool isLeftGun = true;

    public void HandlePlayerInput(ulong senderNetworkID, 
                                Vector2 moveInput,
                                Quaternion rotation,
                                bool isJumping, 
                                bool isBoosting, 
                                bool isAttacking)
    {
        MovePlayer(moveInput, rotation, isJumping, isBoosting, testRB);
        if (isAttacking) PerformPlayerAttack(rotation, testRB.velocity.magnitude);
        if (!IsHost) return;
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (playerObjects[i].OwnerClientId == senderNetworkID)
            {
                MovePlayer(moveInput, rotation, isJumping, isBoosting, playerRigidbodies[i]);
                if (isAttacking) PerformPlayerAttack(rotation, testRB.velocity.magnitude);
            }
        }
    }

    void MovePlayer(Vector2 moveInput, Quaternion rotation, bool isJumping, bool isBoosting, Rigidbody rigidbody)
    {
        Debug.Log(moveInput);
        rigidbody.transform.rotation = rotation;
        if (isBoosting)
        {
            rigidbody.AddForce(rigidbody.transform.forward * moveInput.y * 2, ForceMode.Force);
            rigidbody.AddForce(rigidbody.transform.right * moveInput.x * 2, ForceMode.Force);
            if (isJumping) rigidbody.AddForce(rigidbody.transform.up * 2, ForceMode.Force);
        }
        else
        {
            rigidbody.AddForce(rigidbody.transform.forward * moveInput.y, ForceMode.Force);
            rigidbody.AddForce(rigidbody.transform.right * moveInput.x, ForceMode.Force);
            if (isJumping) rigidbody.AddForce(rigidbody.transform.up * 1, ForceMode.Force);
        }

    }

    void PerformPlayerAttack(Quaternion rotation, float currentVelocity)
    {
        if (canShoot)
        {
            canShoot = false;
            if (isLeftGun)
            {
                GameObject newBullet = Instantiate(bullet, bulletPointOne.position, Quaternion.identity, this.transform);
                newBullet.transform.rotation = bulletPointOne.rotation;
                newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * (40 + currentVelocity);
                Destroy(newBullet, 10);
                isLeftGun = false;
            }
            else
            { 
                GameObject newBullet = Instantiate(bullet, bulletPointTwo.position, Quaternion.identity, this.transform);
                newBullet.transform.rotation = bulletPointTwo.rotation;
                newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * (40 + currentVelocity);
                Destroy(newBullet, 10);
                isLeftGun = true;
            }
            StartCoroutine(WeaponCooldown());
        }
    }

    IEnumerator WeaponCooldown()
    {
        float progress = 0;
        while (progress < 0.1f) 
        {
            progress += Time.deltaTime;
            yield return null;
        }
        canShoot = true;
    }
}
