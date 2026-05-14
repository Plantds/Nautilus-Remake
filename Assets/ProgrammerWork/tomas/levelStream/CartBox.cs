//using UnityEngine;
//
//public class CartBox : MonoBehaviour
//{
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//        
//    }
//    private void OnTriggerEnter(Collider other)
//    {
//        Player player;
//        if (other.TryGetComponent<Player>(out player))
//        {
//            EnteredVan();
//        }
//    }
//    void EnteredVan()
//    {
//        TrainCartLoader.GetInstance().EnteredCart(this);
//    }
//}
