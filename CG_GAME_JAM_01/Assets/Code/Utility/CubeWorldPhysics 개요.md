### CubeWorld Physics 개요

이 문서는 Cube World에서 Physics 업데이트를 위한 전반적인 내용을 설명한다. CubeWorld 게임에서, 사용자가 던질 수 있는 `CGObject` 상속을 받은 것 중 Enum Type이 `MOVABLE` 인 것들에 대한 내용이다. 이 CubeWorld Physics는 나중의 Server에도 들어가야 될 것이지만, 아직은 하지 그것까지 고려하기에 시간이 없으므로, Local에서만 하는 것을 고려한다.



## 주요 업데이트 로직

우선 사용자가 그 Object를 던질 때이다. 우리는 사용자가 바라보고 있는 Direction (Transform의 Front) 방향의 벡터와 사용자가 던지기 버튼을 눌러서 0 ~ 1 사이의 파워 대로 날린다. 이 때, 코드가 아마 이렇게 될 것이다.

```c++
float power;
Vector3 userFront;

Vector3 userTriggerForce = userFront * power * kConstant;

rigidbody.AddForce(userTriggerForce);
```

따라서 FixedUpdate에서는 추가된 Force에 따라, Object의 움직임을 업데이트할 것이다. 그리고 이 때 Update 루틴에서, Physics World Manager를 그 날아가는 오브젝트의 Cube Area를 계속 살펴야 한다. Cube Area A에서 B로 날아갈 때, Cube Physics World Manager는 Fixed Update에서 A에서 B로 변환되는 순간, 특정 Force를계속 주어야 하고, B에서 C (User의 공격 Target Cube Area)로 넘어갈 때, 이 Force를 어떻게 줄지를 결정해야 한다.



따라서 CGObject는 이것과 관련해서 다음과 같은 state들을 가지고 있어야 한다.

* enum CubeAreaEnum PreviousArea
* enum CubeAreaEnum CurrentArea
* bool isAreaChanged

왜냐하면, Physics World Manager(PWM)은 해당 Movable Object가 어떤 Area에서 어떤 Area로 넘어갔는지를 알아야 하기 때문이다. 그리고 우리가 현재 처리하지 않으려는 Area로 던지게 되면, 그걸 Undo 시키는 게 필요해서, 그 CGObject가 배치되었던 곳 위치로 다시 돌아가야 한다.



다시, Update로직으로 돌아가서, CGObject가 isAreaChanged가 true가 되는 순간 PWM은 어떤 방향으로 Force를 줄지, 그리고 얼마나 많은 Force를, 얼마나 오랫동안 줄지를 정해야 한다. 하지만, 이것은  테스트를 통해 어떤 게 적절한지, 아니면 바람과 같은 요소를 넣어서 사용자가 그 바람의 세기를 알 게 해야 하는지 등 여러가지 요소들이 있기 때문에 일단은 단순하게 쎈 Force를 한 번만 주어서 사용자가 원하는 Area에 들어갈만큼 넣어버리게 간단하게 구현한다.



따라서 다음과 같은 정보들이 필요하다.

```
bool[,] m_CanAreaChange = new bool[(uint)CubeAreaEnum.None, (uint)CubeAreaEnum.None];
Vector3[,] m_directionVector = new Vector3[(uint)CubeAreaEnum.None, (uint)CubeAreaEnum.None];
```



따라서 어디서 왔는지에 따라 우리는 어떤 방향으로 보내야 하는지 방향만 알고 있다면, 해당 방향으로 우리가 정한 force constant로 사용자가 target하는 area에 보낼 수 있을 것이다.



이제, 이것을 어떻게 추적할지이다. 사용자가 던지는 순간 CGObject는 `Trigger` 라는 상태를 가지게 되고, PWM의 배열안에 넣어지게 된다. 따라서, PWM은 그 Trigger라는 배열을 보고 FixedUpdate에서 Trigger를 위한 정보를 통해 위에서 정의한 AddForce를사용하여 날려주고, Trigger를 Moving으로 바꿔준다. 그리고나서, Moving 중에는, Area 변화상태를 체크하여, 변화된다면, m_directionVector를 통해 force를 주어 사용자가 원하는 target area에 날아가도록 한다. 그 때 force를 준다면, 그 object의 moving_with_force_field state가 되고, force를 더 이상 주지 않는다. 그리고, 그 object가 target area에 도달했다면, gravity를 해당 area 공간의 gravity로 바꿔주어 행동하게 해준다. 이 때는 PWM이 할 일이 없으므로, PWM의 배열에서 제외 시킨다. 현재는 턴제이기 때문에, 하나의 오브젝트에 대해서만 이 로직을 적용해도 되지만, 배열이라고 말한 이유는 실시간으로 여러 오브젝트를 던지는 모드의 경우에는 그러한 배열들로 처리해야 할 것이기 때문이다. 현재 state에 대한 enum은 코딩하면서 다시 따로 정리할 것이다.



## 충돌 탐지 Layer

현재 우리는 Player가 자신의 Area를 빠져나가지 못하게, Plane에 대해 BoxCollider를 사용하여 막고 있다. 하지만 이것을 그대로 둔다면, 사용자가 던지는 Movable Object는 여기에 막힐 것이다. 따라서, Layer를 설정하여 Movable Object들은 이 Plane들과 충돌탐지를 하지 않도록 설정할 것이다. Layer들은 다음과 같이 구분된다.

* CGObject_UnMovable
  * 여기에는 Player/Decoration Object/House 등이 포함된다.
* CGObject_Movable
  * 여기에는 플레이어가 던질 수 있는 Object가 포함된다.
* CGWorld_PlayerBarrier
  * 플레이어가 나가지 못하게 하는 Plane들이다.
* CGWorld
  * 플레이어가 서있는 Cube이다.

우리는 현재 CGObject_Movable과 CGWorld_PlayerBarrier사이의 충돌 탐지를 하지 않아야 한다. 그 외에는 다른 Layer들은 모두 충돌탐지를 한다.