 public enum bulletType
    {
        solid,
        explosive
    }
    // Start is called before the first frame update

    public float explosionRadius = 10f;
    public float explosivePower = 10f;
    public float rawDamage = 50;
    public float penetrationCapability;
    public float bulletSize = 1f;
    public float destroyTime = 10f;
    public float ricochetProbability = 60;
    public float ricochetAngle = 70;
    public float penetrationDecreasePer100Meters = 2;
    public bulletType typeOfBullet;

    bool canDamage = true;

    Vector3 lastPosition;
    float distanceTravelled = 0;
    public void activate()
    {
        Destroy(gameObject, destroyTime);
        lastPosition = transform.position;
    }


    private void Update()
    {
        distanceTravelled += Vector3.Distance(lastPosition, transform.position);
        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EffectHolder.instance.createEffect("shootImpact", collision.contacts[0].point, collision.collider.transform, collision.contacts[0].normal, 5f, new Vector3(bulletSize, bulletSize, bulletSize));

        if (canDamage && collision.collider.GetComponent<Armor>())
        {
            Armor collided = collision.collider.GetComponent<Armor>();

            bool ricochet = false;
            Vector3 impactPoint = collision.contacts[0].point;

            EffectHolder.instance.createEffect("metalImpact", impactPoint, collision.collider.transform, collision.contacts[0].normal, 5f, new Vector3(bulletSize, bulletSize, bulletSize));

            if (typeOfBullet == bulletType.explosive)
            {
                EffectHolder.instance.createEffect("explosion", impactPoint, collision.collider.transform, collision.contacts[0].normal, 5f, new Vector3(bulletSize, bulletSize, bulletSize));
            }

            float losArmor;
            float impactangle;
            impactangle = Vector3.Angle(collision.contacts[0].normal, transform.position);
            impactangle -= 180;
            impactangle = Mathf.Abs(impactangle);

            losArmor = collided.thickness / Mathf.Sin(impactangle);
            losArmor = Mathf.Abs(losArmor);

            if (impactangle > ricochetAngle)
            {
                if (Random.Range(0, 100) < ricochetProbability)
                {
                    ricochet = true;
                }
            }
            Debug.Log("LOS Armor: " + losArmor + "Impact Angle: " + impactangle);
            bool continueDamage = true;
            if (collided.GetType() == typeof(TankComponent))
            {
                TankComponent component = (TankComponent)collided;
                component.damageComponent();
                continueDamage = false;
            }
            if (continueDamage && !ricochet)
            {
                float penetrationHolder = penetrationCapability;
                if (collided.GetType() == typeof(ERAArmor))
                {
                    ERAArmor era = (ERAArmor)collided;
                    if (era.durability > 0)
                    {
                        float eraEfficiency = 0;
                        float penetrationReduce;

                        if (typeOfBullet == bulletType.solid)
                        {
                            eraEfficiency = era.durability / era.originalDurability * era.KEEfficiency;
                        }
                        else if (typeOfBullet == bulletType.explosive)
                        {
                            eraEfficiency = era.durability / era.originalDurability * era.HEEfficiency;
                        }
                        penetrationReduce = penetrationHolder * (eraEfficiency / 100);
                        penetrationHolder -= penetrationReduce;
                        era.reduce();
                    }
                }


                float penetrationDifference = penetrationHolder / penetrationCapability;
                float compositeReduce = 0;

                if (typeOfBullet == bulletType.solid)
                {
                    float Travelled100Units = distanceTravelled / 100;
                    float travelDecrease = Travelled100Units * penetrationDecreasePer100Meters;
                    penetrationHolder -= travelDecrease;
                    compositeReduce = collided.KEReduction / 100 * penetrationHolder;
                    rawDamage -= penetrationDifference * 2;
                }
                else if (typeOfBullet == bulletType.explosive)
                {
                    compositeReduce = collided.HEReduction / 100 * penetrationHolder;
                }
                penetrationHolder -= compositeReduce;

                if (losArmor < penetrationHolder)
                {
                    collided.registerDamage(rawDamage);
                }

            }
            if (ricochet == false)
            {

                Destroy(gameObject, 0f);
            }
            else
            {

                GetComponent<Collider>().enabled = false;
                Destroy(gameObject, 3f);
            }
            canDamage = false;
        }



    }
