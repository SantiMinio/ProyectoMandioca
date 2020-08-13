﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tools.Extensions;
using Tools.EventClasses;

public class CustomCamera : MonoBehaviour
{
    public Transform target;
    public Transform charTransform;
    private Vector3 velocity = Vector3.zero;
    public float smooth = 1f;
    public Vector3 offset;
    public bool lookAt;
    public float shakeAmmount;
    float original_shake_amount;
    private float shakeDurationCurrent;
    public float shakeDuration;
    private bool activeShake;
    public bool active = true;
    public float horizontalSpeed;
    public float verticalSpeed;
    [SerializeField] float _speedAwayOfMesh;
    public float horizontal;
    public float vertical;

    Collider currentObstacle;
    [SerializeField] float sensitivity;
    [SerializeField] private SkillCloseUp_Camera skillCloseUp_Camera = null;

    public float zoomDuration;
    const int FIELD_OF_VIEW_ORIGINAL = 60;
    float fieldOfView_toZoom = 60;

    PingPongLerp pingpongZoom = new PingPongLerp();

    public LayerMask _layermask_raycast_mask;
    [SerializeField, Range(0, 1)] float fade = 0.05f;
    public float speedRot;
    Camera mycam;
    bool setOverTheSholder;
    public List<CamConfiguration> myCameras = new List<CamConfiguration>();
    public CamConfiguration overTheSholderCam;
    public bool activateOverTheSholder;
    public int index;
    JoystickBasicInput _joystick;
    float startHorizontal;
    float StartVertical;
    [SerializeField] RotateTheCamera _rotOfCamera;

    [SerializeField] EventInt invertAxis;

    private void Start()
    {
        //_joystick = new JoystickBasicInput();
        //_joystick.SUBSCRIBE_START(ChangeCamera);
        shakeDurationCurrent = shakeDuration;
        mycam = GetComponent<Camera>();
        pingpongZoom.Configure(Zoom, false);
        changeCameraconf(0);
        original_shake_amount = shakeAmmount;
        charTransform = Main.instance.GetChar().GetLookatPosition();
        //skillCloseUp_Camera.SubscribeOnTurnOnCamera(CloseToCharacter);
        //skillCloseUp_Camera.SubscribeOnTurnOnCamera(ExitToCharacte);
    }

    #region Close Camera
    public void DoCloseCamera() => skillCloseUp_Camera.TurnOnSkillCamera();
    public void DoExitCamera() => skillCloseUp_Camera.TurnOffSkillCamera();
    void CloseToCharacter() { }
    void ExitToCharacte() { }
    #endregion

    private void Update()
    {
        if (!active)
            return;
        if (activateOverTheSholder)
        {
            OverTheSholder();
            return;
        }

        pingpongZoom.Updatear();
        ShaderMask();
        if(!lookAt)
        transform.forward = Vector3.Lerp(transform.forward, myCameras[index].transform.forward, speedRot * Time.deltaTime);
       // _joystick.Refresh();
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    NextCamera();
        //}
    }
    private void FixedUpdate()
    {
        if (!active || activateOverTheSholder)
            return;
        SmoothToTarget();
    }
    private void LateUpdate()
    {
        if (!active || activateOverTheSholder)
            return;
        if (activeShake) Shake();
    }

    public void DoFastZoom(float _speedanim, float _fieldOfViewToZoom = 55)
    {
        fieldOfView_toZoom = _fieldOfViewToZoom;
        pingpongZoom.Play(_speedanim);
    }
    void Zoom(float valtozoom) => mycam.fieldOfView = Mathf.Lerp(FIELD_OF_VIEW_ORIGINAL, fieldOfView_toZoom, valtozoom);
    void SmoothToTarget()
    {
        Vector3 desiredposition = target.position + offset;
        float axisX = Input.GetAxis("Horizontal");
        float axisZ = Input.GetAxis("Vertical");
        Vector3 moveOffset = desiredposition;
        if (axisX != 0)
        {
            moveOffset += transform.right * axisX * sensitivity;
        }
        if (axisZ != 0)
        {
            moveOffset += transform.up * axisZ * sensitivity;
        }
        Vector3 smoothedposition = Vector3.Lerp(transform.position, moveOffset, smooth * Time.deltaTime);
        transform.position = smoothedposition;
        if (lookAt) transform.LookAt(charTransform);
    }
    public void InstantPosition()
    {
        transform.position = target.position + offset;
    }
    void ShaderMask()
    {
        /////////////////////////////////////////////////////////////////////////////
        //// HACER QUE ESTO FUNQUE
        /////////////////////////////////////////////////////////////////////////////


        RaycastHit hit;
        var dir = Main.instance.GetChar().transform.position - this.transform.transform.position;
        dir.Normalize();
        if (Physics.Raycast(this.transform.transform.position, dir, out hit, 10000, _layermask_raycast_mask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.GetComponent<MeshRenderer>())
            {
                if (currentObstacle != hit.collider)
                {
                    currentObstacle?.GetComponent<MeshRenderer>().material.SetFloat("_Intensity", 1);
                    currentObstacle = hit.collider;
                    StartCoroutine(ShaderFade());
                }
            }
            else
            {
                currentObstacle?.GetComponent<MeshRenderer>()?.material.SetFloat("_Intensity", 1);
                currentObstacle = null;
                StopCoroutine(ShaderFade());
            }
            DebugCustom.Log("CameraThings", "Raycast Hit Element", hit.transform.gameObject.name);

            //para que era esto?
            //Main.instance.GetChar().Mask(!hit.transform.GetComponent<CharacterHead>());

        }
        else
        {
            currentObstacle?.GetComponent<MeshRenderer>().material.SetFloat("_Intensity", 1);
            currentObstacle = null;
            StopCoroutine(ShaderFade());
        }
    }


    IEnumerator ShaderFade()
    {
        float currentFade = 1f;

        while (true)
        {
            currentFade -= 0.1f;
            yield return new WaitForSeconds(fade);
            if (currentObstacle && currentFade > 0.3f) currentObstacle.GetComponent<MeshRenderer>().material.SetFloat("_Intensity", currentFade);
            else break;
        }
    }

    public void BeginShakeCamera(float shake = -1)
    {
        shakeAmmount = shake != -1 ? shake : original_shake_amount;
        activeShake = true;
        shakeDurationCurrent = shakeDuration;

    }
    private void Shake()
    {
        if (shakeDurationCurrent > 0)
        {
            transform.position += Random.insideUnitSphere * shakeAmmount;
            shakeDurationCurrent -= Time.deltaTime;
        }
        else
        {
            shakeDurationCurrent = 0;
            activeShake = false;
        }
    }
    void SmoothDump() => transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smooth);


    public void NextCamera()
    {
        index = index.NextIndex(myCameras.Count);
        changeCameraconf(index);
        invertAxis.Invoke(index);
    }
    public void PrevCamera()
    {
        index = index.BackIndex(myCameras.Count);
        changeCameraconf(index);
        invertAxis.Invoke(index);
    }

    //private void nextIndex()
    //{
    //    //cambiar lo de poner el input. es solo para probar
    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        if (index < myCameras.Count - 1)
    //            index++;
    //        else
    //            index = 0;
    //        changeCameraconf(index);
    //    }


    //}
    void ChangeCamera()
    {
        if (index < myCameras.Count - 1)
            index++;
        else
            index = 0;
        changeCameraconf(index);
    }
    void changeCameraconf(int i)
    {
        target = myCameras[i].transform;
        shakeAmmount = myCameras[i].shakeAmmount;
        shakeDuration = myCameras[i].shakeDuration;
        smooth = myCameras[i].smoothTime;

        Camera camera = GetComponent<Camera>();
        camera.cullingMask = myCameras[i].CullingMask;
        camera.fieldOfView = myCameras[i].fieldOfView;
    }

    public void ChangeToDefaultCamera()
    {
        active = true;
        index = 0;
        changeCameraconf(index);
        invertAxis.Invoke(index);
        if(!lookAt)
        transform.forward = Vector3.Lerp(transform.forward, myCameras[index].transform.forward, speedRot * Time.deltaTime);
    }
    void OverTheSholder()
    {
        if (!setOverTheSholder)
        {
            target = overTheSholderCam.transform;
            shakeAmmount = overTheSholderCam.shakeAmmount;
            shakeDuration = overTheSholderCam.shakeDuration;
            smooth = overTheSholderCam.smoothTime;

            Camera camera = GetComponent<Camera>();
            camera.cullingMask = overTheSholderCam.CullingMask;
            camera.fieldOfView = overTheSholderCam.fieldOfView;
            setOverTheSholder = true;
            transform.forward = overTheSholderCam.transform.forward;
            horizontal = overTheSholderCam.transform.parent.eulerAngles.y;
            vertical = overTheSholderCam.transform.parent.eulerAngles.x;
            Debug.Log(overTheSholderCam.transform.parent.eulerAngles.y);
            startHorizontal = horizontal;
            StartVertical = vertical;
        }

        transform.position = Vector3.Lerp(transform.position, overTheSholderCam.transform.position, Time.deltaTime * smooth);

        horizontal += horizontalSpeed * Input.GetAxis("Horizontal");
        horizontal = Mathf.Clamp(horizontal, (startHorizontal - 45), (startHorizontal + 45));
        vertical += verticalSpeed * Input.GetAxis("Vertical");
        vertical = Mathf.Clamp(vertical, (StartVertical-45), (StartVertical+45));

        transform.rotation = Quaternion.Euler(-vertical, horizontal, 0);

    }

    public void inputToOverTheSholder(bool active)
    {

        if (active)
        {
            activateOverTheSholder = false;
            setOverTheSholder = false;
            ChangeToDefaultCamera();
            //horizontal = 0;
            //vertical = 0;
        }
        else
        {
            activateOverTheSholder = true;
        }


    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 21)
            _rotOfCamera.RotationOfCamera(_speedAwayOfMesh);
    }
}
