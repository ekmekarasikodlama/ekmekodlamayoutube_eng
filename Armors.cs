 public float thickness;
    [HideInInspector]
     public Entity parentScript;

    public float KEReduction = 0; 
    public float HEReduction = 30;

    private void Awake()
    {
        parentScript = transform.root.GetComponent<Entity>();
    }
    public virtual void registerDamage(float amount)
    {
        amount = Mathf.Abs(amount);
     
        Debug.Log("Armor Name: " + name + " Damage Amount: " + amount);
        parentScript.takeDamage(amount);
    }
    
    
    // Entity = your damage holding code
    
    
    public class ERAArmor : Armor
{
    public float KEEfficiency = 0;
    public float HEEfficiency = 50;

    public float durability = 100;
    public float originalDurability = 100;
    public void reduce(float divider = 1)
    {
        durability -= 80/divider;
    }

}

public enum components
{
    barrel,
    optics
}
public class TankComponent : Armor
{
    public components representitiveComponent;
    public void damageComponent()
    {
        Debug.Log("Damaged component: " + representitiveComponent.ToString());
    }
}
