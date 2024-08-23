using System.Collections;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 500;
    private bool _isSpeeding = false;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10; // how hard to hit enemy without power up.
    private float powerupStrength = 25; // how hard to hit enemy with power up.
    
    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    private void Update()
    {
        // Add force to player in direction of the focal point (and camera).
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * (verticalInput * speed * Time.deltaTime)); 

        // Set powerup indicator position to beneath player.
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);
        
        // Gives the player a boost when Space is pressed.
        if (Input.GetKey(KeyCode.Space))
        {
            _isSpeeding = true;
            speed = 800;
        }
        else
        {
            _isSpeeding = false;
            speed = 500;
        }
    }

    // If Player collides with power up, activate power up.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
    }

    // Coroutine to count down power up duration.
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy.
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            var awayFromPlayer =  (transform.position + other.gameObject.transform.position).normalized; 
           
            if (hasPowerup) // if have power up hit enemy with power up force.
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no power up, hit enemy with normal strength.
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }


        }
    }



}
