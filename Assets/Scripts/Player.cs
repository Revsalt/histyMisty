using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Player : MonoBehaviour
{ 
    public static Player instance;

    [Header("Movement")]
    public float gravity = -9.81f;
    public float speed;
    public float sprintSpeed;
    public LayerMask groundLayer;
    public float jumpHeight;
    [Header("PlayerAttributesDelay")]
    public float dashDelay;
    public float spinDelay;
    public int DashCount = 3;
    [Header("HeadBobbing")]
    public float headBobRadius = 0.5f;
    public float headBobY = 1;
    [Header("ExtraSkills")]
    public float _dashTime = 0;
    public float _dashSpeed;
    [Header("Visuales / Other Variables")]
    public GameObject jumpTrail;
    public GameObject Mesh;
    public GameObject Bullet;

    private Animator animator;
    private CharacterController controller;
    private Vector3 Impact = Vector3.zero;
    private bool jumped = false;
    GameObject modelDummyRotation;
    [HideInInspector]
    public float yVelocity = 0;
    private float _speed;
    private bool canMove = true;
    private bool canjump;

    //Atrributes
    bool isDash = true;
    bool isSpin = false;

    Color futurecolor;
    Material hatMaterial;
    int stamina;

    private void Start()
    {
        instance = this;

        modelDummyRotation = new GameObject("modelDummyRotation");

        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        speed *= Time.fixedDeltaTime;
        sprintSpeed *= Time.fixedDeltaTime;
        jumpHeight *= Time.fixedDeltaTime;

        hatMaterial = Mesh.GetComponentInChildren<SkinnedMeshRenderer>().materials[4];
        futurecolor = hatMaterial.color;

        SetGlow(6, hatMaterial);
        stamina = DashCount;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isMoving())
        {
            _speed = sprintSpeed;
            MouthConfiguration.instance.SetMouth("o_Mouth");
        }
        else
        {
            _speed = speed;
            MouthConfiguration.instance.SetMouth("normal_Mouth");
        }

        hatMaterial.SetColor("_AmbientColor", Color.Lerp(hatMaterial.GetColor("_AmbientColor"), futurecolor, 5 * Time.deltaTime));

        //

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isDash)
                return;

            Vector3 dashDirection = transform.forward;

            dashDirection = transform.forward;

            StartCoroutine(DashCoroutine(dashDirection));

            stamina -= 1;
            SetGlow(-1.2f , hatMaterial);

            //CameraEffects
            CameraShaker.Instance.ShakeOnce(5, 15, 0.1f, 0.5f);
            Camera.instance.SetFeildofview(70, 100);

            if (stamina <= 0)
                StartCoroutine(DashCoroutineDelay());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isSpin)
                return;

            StartCoroutine(SpinDelay(spinDelay));

            //CameraEffects / VisualEffects
            CameraShaker.Instance.ShakeOnce(10, 20, 0.1f, 1);
            PlayerParticles.Instance.spinParticleSystem.Play();

            //DealingDamage
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.GetComponent<Health>() != null && hitCollider.GetComponent<Health>() != GetComponent<Health>())
                {
                    hitCollider.GetComponent<Health>().TakeDamage(20);
                }
            }

            //Glowing
            AddGlow(2 , Mesh.GetComponentInChildren<SkinnedMeshRenderer>());
        }

        canjump = Input.GetKey(KeyCode.Space);

        if (Impact.magnitude > 0.2) controller.Move(Impact * Time.deltaTime);
        Impact = Vector3.Lerp(Impact, Vector3.zero, 5 * Time.deltaTime);

        //HeadBob

        if (Physics.CheckSphere(transform.position + new Vector3(0,headBobY , 0) , headBobRadius , groundLayer))
        {
            AddImpact(-8 , -Vector3.up);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, headBobY, 0), headBobRadius);
    }

    private IEnumerator SpinDelay(float spindelay)
    {
        isSpin = true;

        animator.SetBool("isSpinAttack", true);

        canMove = false;
        yield return new WaitForSeconds(0.1f);
        canMove = true;

        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isSpinAttack", false);

        ResetGlow(Mesh.GetComponentInChildren<SkinnedMeshRenderer>());

        yield return new WaitForSeconds(spindelay);
        isSpin = false;
    }

    private IEnumerator DashCoroutineDelay()
    {
        byte k_MaxByteForOverexposedColor = 191; //internal Unity const
        Color _emissionColor = hatMaterial.GetColor("_AmbientColor");
        var maxColorComponent = _emissionColor.maxColorComponent;
        var scaleFactor = k_MaxByteForOverexposedColor / maxColorComponent;
        float intensity = Mathf.Log(255f / scaleFactor) / Mathf.Log(2f);


        isDash = false;
        hatMaterial.SetColor("_AmbientColor", hatMaterial.GetColor("_AmbientColorStatic"));

        yield return new WaitForSeconds(dashDelay / 6);
        SetGlow(2, hatMaterial);
        yield return new WaitForSeconds(dashDelay / 6);
        SetGlow(-2, hatMaterial);
        yield return new WaitForSeconds(dashDelay / 6);
        SetGlow(2, hatMaterial);
        yield return new WaitForSeconds(dashDelay / 6);
        SetGlow(-10, hatMaterial);
        yield return new WaitForSeconds((dashDelay / 6) * 2);

        hatMaterial.SetColor("_AmbientColor", hatMaterial.GetColor("_AmbientColorStatic"));
        isDash = true;
        stamina = 3;
        SetGlow(6, hatMaterial);
    }

    private IEnumerator DashCoroutine(Vector3 direction)
    {
        AddGlow(5, Mesh.GetComponentInChildren<SkinnedMeshRenderer>());

        GetComponent<SC_MovingPlaftform>().activePlatform = null;
        PlayerParticles.Instance.dashParticleSystem.transform.LookAt(transform.position + -direction * 3);
        PlayerParticles.Instance.dashParticleSystem.Play();

        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(SummonBody(i * 0.03f));
        }

        //dashing

        float startTime = Time.time; // need to remember this to know how long to dash
        while (Time.time < startTime + _dashTime)
        {
            controller.Move(direction * _dashSpeed * Time.deltaTime);
            // or controller.Move(...), dunno about that script
            yield return null; // this will make Unity stop here and continue next frame
        }

        //falshy looks

        Camera.instance.SetFeildofview(63.6f, 5);
        ResetGlow(Mesh.GetComponentInChildren<SkinnedMeshRenderer>());
        yVelocity = 0f;

        IEnumerator SummonBody(float time)
        {
            yield return new WaitForSeconds(time);
            Animator anim = Instantiate(Mesh, Mesh.transform.position, Mesh.transform.rotation, null).GetComponent<Animator>();
            anim.enabled = false;

            AddGlow(0.5f , anim.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());

            Destroy(anim.gameObject, 0.3f - time * 2);
            StopCoroutine(SummonBody(time));
        }
    }

    Vector3 velo;
    public void FixedUpdate()
    { 
        Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), _speed);

        //model rot

        modelDummyRotation.transform.position = transform.position;

        Vector3 v = transform.position + Input.GetAxis("Horizontal") * Camera.instance.transform.right + Input.GetAxis("Vertical") * Camera.instance.transform.forward;

        modelDummyRotation.transform.LookAt(v);

        if (isMoving())
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.Euler(0, modelDummyRotation.transform.eulerAngles.y, 0), 20 * Time.deltaTime);
        }

        //sexy stuff

        jumpTrail.SetActive(!OnGrounded());

        animator.SetBool("isRun", Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isJump", !OnGrounded());
        animator.SetFloat("JumpDist", GetDistanceToground());
        animator.SetBool("isWalk", isMoving() && OnGrounded());
         
        var main = PlayerParticles.Instance.smokeParticleSystem.main;

        main.simulationSpeed = _speed * 5;
        if (isMoving() && OnGrounded())
        {
            if (!PlayerParticles.Instance.smokeParticleSystem.isPlaying)
                PlayerParticles.Instance.smokeParticleSystem.Play();
        } else if (PlayerParticles.Instance.smokeParticleSystem.isPlaying){

            if (PlayerParticles.Instance.smokeParticleSystem.isPlaying)
                PlayerParticles.Instance.smokeParticleSystem.Stop(); 
        }
    }

    public void SetKnockBack(float _knockback ,Vector3 direction , float _Upwardsknockback)
    {
        AddImpact(_knockback, direction);
        AddImpact(_Upwardsknockback, Vector3.up);
    }

    void AddImpact(float force,Vector3 direction)
    {
        direction.Normalize();
        if (direction.y < 0) direction.y = -direction.y; // reflect down force on the ground
        Impact += direction.normalized * force / 1;
    }

    private void Move(Vector2 _inputDirection , float _speed)
    {
        if (!canMove)
            return;


        Vector3 _moveDirection = Camera.instance.transform.right * _inputDirection.x + Camera.instance.transform.forward * _inputDirection.y;
        _moveDirection *= _speed;

        if (OnGrounded())
        {
            yVelocity = 0f;
            if (canjump)
            {
                yVelocity = jumpHeight;
                GetComponent<SC_MovingPlaftform>().activePlatform = null;
                StartCoroutine(JumpDelay());
            }
        }

        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        if (jumped && OnGrounded())
        {
            PlayerParticles.Instance.landParticleSystem.Play();
            CameraShaker.Instance.ShakeOnce(4, 3, 0.1f, 0.5f);
            jumped = false;
        }

        IEnumerator JumpDelay()
        {
            yield return new WaitForSeconds(0.07f);
            jumped = true;
        }
    }

    public bool isMoving()
    {
        return (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
    }

    public float GetDistanceToground()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.up * -1, out hit);

        return Vector3.Distance(transform.position, hit.point);
    }

    public bool OnGrounded()
    {
        return
        Physics.CheckCapsule(controller.bounds.center, new Vector3(controller.bounds.center.x,
            controller.bounds.min.y, controller.bounds.center.z), controller.radius * .4f, groundLayer);
    }

    public void AddGlow(float value , SkinnedMeshRenderer mats)
    {
        if (value > 0)
        {
            value = Mathf.Abs(value);

            foreach (var item in mats.materials)
            {
                if (item == hatMaterial)
                    return;

                Color color_ = item.GetColor("_AmbientColor");

                float factor = Mathf.Pow(2, value);
                Color color = new Color(color_.r * factor, color_.g * factor, color_.b * factor);
                item.SetColor("_AmbientColor", color);
            }
        }
        else if (value < 0)
        {
            value = Mathf.Abs(value);

            foreach (var item in mats.materials)
            {
                if (item == hatMaterial)
                    return;


                Color color_ = item.GetColor("_AmbientColor");

                float factor = Mathf.Pow(2, value);
                Color color = new Color(color_.r / factor, color_.g / factor, color_.b / factor);
                item.SetColor("_AmbientColor", color);
            }
        }
        else
            return;
    }

    public void ResetGlow(SkinnedMeshRenderer mats)
    {
            foreach (var item in mats.materials)
            {
                if (item == hatMaterial)
                    return;

                item.SetColor("_AmbientColor", item.GetColor("_AmbientColorStatic"));
            }
    }

    public void SetGlow(float value, Material mats)
    {
        if (value > 0)
        {
            Color color_ = mats.GetColor("_AmbientColor");

            float factor = Mathf.Pow(2, value);
            Color color = new Color(color_.r * factor, color_.g * factor, color_.b * factor);
            futurecolor = color;
        }
        else if (value < 0)
        {
            value = Mathf.Abs(value);

            Color color_ = mats.GetColor("_AmbientColor");

            float factor = Mathf.Pow(2, value);
            Color color = new Color(color_.r / factor, color_.g / factor, color_.b / factor);
            futurecolor = color;
        }
    }
}
