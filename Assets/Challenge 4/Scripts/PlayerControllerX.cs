using System.Collections;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 500;
    private GameObject focalPoint;
    public ParticleSystem speedParticleSystem;

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
        
        // Position the particle system behind the player.
        Vector3 particlePosition = transform.position - focalPoint.transform.forward * 1.5f; // Adjust the distance behind the player as needed.
        speedParticleSystem.transform.position = particlePosition;
        
        // Gives the player a boost when Space is pressed.
        if (Input.GetKey(KeyCode.Space))
        {
            speed = 800;
            // Checks if the system is already playing so that it isn't restarted every frame.
            if (!speedParticleSystem.isPlaying)
            {
                speedParticleSystem.Play();
            }
        }
        else
        {
            speed = 500;
            if (speedParticleSystem.isPlaying)
            {
                speedParticleSystem.Stop();
            }
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
