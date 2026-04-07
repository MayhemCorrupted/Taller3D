using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] int walkSpeed = 5;
    CharacterController controler;
    private void Awake()
    {
        controler = GetComponent<CharacterController>();
    }
    void Start()
    {
        
    }

    void Update()
    {
       
    }
}
