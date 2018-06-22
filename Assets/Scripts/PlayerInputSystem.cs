using Unity.Entities;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem{
    struct Filter {
        public PlayerInput Input;
    }

    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;

        foreach (var entity in GetEntities<Filter>())
        {
            var pi = entity.Input;

            pi.Move.x = Input.GetAxis("Horizontal");
            pi.Move.y = Input.GetAxis("Vertical");
          
            if (Input.GetButton("Fire1")) {
              pi.Shoot = true;
            } else {
              pi.Shoot = false;
            }
        }
    }
}