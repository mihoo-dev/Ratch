using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;




public class SimpleCharacterControl : MonoBehaviour {


    //----------------------------------------------------------
    // 아이템 매니저 생성
    //----------------------------------------------------------
    public ItemManager IM;

    private enum ControlMode
    {
        Tank,
        Direct
    }

    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();




private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }



        //----------------------------------------------------------
        // 기능: 적과의 충돌 부분 ( First Person Player 스크립트에서 가져옴)
        //----------------------------------------------------------
        
    
            if (collision.gameObject.name == "eyes")
            {
                collision.transform.parent.GetComponent<monster>().CheckSight();
            }
        


        //----------------------------------------------------------
        // 기능: 오브젝트들(열쇠, 아이템, 문)과 캐릭터 충돌시에 이벤트를 처리
        //----------------------------------------------------------




        string ObjectCollision = collision.transform.tag; // 캐릭터와 충돌한 오브젝트의 tag를 저장하는 string변수
  
        switch (ObjectCollision)
        {

             //--------------------------------------------------------------------------------------------
             // 캐릭터 - 아이템&열쇠 충돌부분
             //--------------------------------------------------------------------------------------------
            case "Key0":  // 열쇠0과 충돌 시,

                collision.gameObject.SetActive(false); // 열쇠0과 충돌 후 열쇠0 사라지게함.
                IM.Get_KeyNo++; // 열쇠0을 얻었으니 열쇠얻은 수 +1
                Debug.Log("Key0 습득!");
                IM.Get_Key[0] = true; // 열쇠0의 습득여부를 True로 바꿔줌.

                break;

            case "Key1":

                collision.gameObject.SetActive(false);
                IM.Get_KeyNo++;
                Debug.Log("Key1 습득!");
                IM.Get_Key[1]= true;
                

                break;

            case "Key2":

                collision.gameObject.SetActive(false);
                IM.Get_KeyNo++;
                Debug.Log("Key2 습득!");
                IM.Get_Key[2] = true;
                break;

            case "Item0":

                collision.gameObject.SetActive(false);
                IM.Get_ItemNo++;
                Debug.Log("Item0 습득!");
                IM.Get_Item[0] = true;

                break;

            case "Item1":

                collision.gameObject.SetActive(false);
                IM.Get_ItemNo++;
                Debug.Log("Item1 습득!");
                IM.Get_Item[1] = true;

                break;

            case "Item2":

                collision.gameObject.SetActive(false);
                IM.Get_ItemNo++;
                Debug.Log("Item2 습득!");
                IM.Get_Item[2] = true;

                break;

            //--------------------------------------------------------------------------------------------
            // 캐릭터-문 충돌부분
            //--------------------------------------------------------------------------------------------

            case "Door0": // 문0과 충돌 시,
                

               if (IM.Get_Key[0] == true) // Key0의 습득여부가 True라면
                {
                    Debug.Log("Key0을 습득했기 때문에 문이 열린다.");
                    collision.gameObject.SetActive(false); // 문0을 사라지게함.

                }

               else //Key0을 습득하지 않았다면
                    Debug.Log("Key0을 모아오세요");

                break;


            case "Door1": // 문1과 충돌 시,


                if (IM.Get_Key[1] == true) 
                {
                    Debug.Log("Key1을 습득했기 때문에 문이 열린다.");
                    collision.gameObject.SetActive(false);

                }

                else //Key1을 습득하지 않았다면
                    Debug.Log("Key1을 모아오세요");

                break;


            case "Door2": // 문2와 충돌 시,


                if (IM.Get_Key[2] == true)
                {
                    Debug.Log("Key2를 습득했기 때문에 문이 열린다.");
                    collision.gameObject.SetActive(false);

                }

                else //Key2을 습득하지 않았다면
                    Debug.Log("Key2를 모아오세요");

                break;




            //--------------------------------------------------------------------------------------------
            // 다음스테이지로 가는 문과의 충돌부분
            //--------------------------------------------------------------------------------------------

            case "Finish_Door":

                if (IM.Get_KeyNo == 3 && IM.Get_ItemNo == 3) // 얻은 열쇠의 수와 아이템 수가 조건을 충족한다면,
                {
                    SceneManager.LoadScene("2"); // 다음 씬으로 넘어갑니다.
                }

                break;

        }


    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

	void Update () {
        m_animator.SetBool("Grounded", m_isGrounded);

        switch(m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            case ControlMode.Tank:
                TankUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }

        m_wasGrounded = m_isGrounded;
    }

    private void TankUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        bool walk = Input.GetKey(KeyCode.LeftShift);

        if (v < 0) {
            if (walk) { v *= m_backwardsWalkScale; }
            else { v *= m_backwardRunScale; }
        } else if(walk)
        {
            v *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

        m_animator.SetFloat("MoveSpeed", m_currentV);

        JumpingAndLanding();
    }

    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Transform camera = Camera.main.transform;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if(direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }

        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }


}
