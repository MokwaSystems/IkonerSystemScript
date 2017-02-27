using UnityEngine;
using System.Collections;

public class IkonerSystem : MonoBehaviour {
	
	/* 
	Skrypt nie jest z tych zaawansowanych. Bardziej chodzi o pokazanie jak z prostych krotków i przemyślenia sprawy można uzyskać
	ciekawy efekt. Efekt dynamiczny i nowoczesny. Przez uzycie odpowiednich grafik do wyświetlania informacji - możemy to jeszcze bardziej ulepszyć.
	
	Skrypt przedstawia mechanizm przedstawienia informacji dla gracza w postaci pojawiającego się napisu, ikony czy obrazka.
	Nie jest to nic odkrywczego - jednak są dwa założenia które dotyczą tego mechanizmu. 
	1) Miał umożliwiać albo utworzenie na starcie poziomu (czyli coś w rodzaju "czasu rzeczywistego")
		obiektu do przedstawienia informacji. Lub też skorzystanie z wczesniej na "sztywno" utworzonego.
		Miał też to sam sprawdzać.
	2) Chodziło o dynamiczne i interaktywne przedstawienie informacji. Ikonka, napis, obrazek itp - miał się pojawiać 
		w warunkach określanych w innym skrypcie. A ten skrypt ma być swego rodzaju API umożliwiającym takie działania.
		Przy czym sposób wyświetlania jest określany tutaj (i są to zmienne publiczne). Chodziło o uzyskanie efektu
		przejścia od pojawienia się, wyświetlania i zanikania - w dynamiczny sposób.
	
	Niektóre zmienne nie są opisane ponieważ istnieją tylko dla danego projektu.
	Bardziej liczy się mechanizm i sposób działania.
	
	Krótko przedstawiając same użycie mechanizmu: 
	Pojawiał się tworzony na starcie poziomu Cube który nie jest wypełniony niczym. Zmienna IndexIkonerObj decyduje o tym czy 
	Cube ma być tworzony w czasie rzeczywistym czy też skrypt ma korzystać z wcześniej utworzonego obiektu na scenie. Domyślnie
	zmienna jest ustawiona na tworzenie na starcie. Ta zamienna nie jest tylko zmienną decyzyjną. Jej wartość przechowuje index w hierarchii 
	obiektu który ma wyświetlać informacje - jeśli został wcześniej utworzony. Index musi być dodatni, stąd jeśli ta zmienna ma wartość ujemną
	równą -1 - to znak dla skryptu, że ma twrzyć obiekt. Po utworzeniu Cuba (możliwa konfiguracja parametrów - jednak domyślne powinny wystarczyć),
	tekstura z odpowiednią informacją zostaje nałożona. Obiekt nie jest wyświetlany, a jego rozmiar jest zerowy. 
	Następnie gdy nastąpi wywołanie, Cube powiększa swój rozmiar. Jeśli nie jest już potrzebna informcja - obiekt zanika - zmniejszając swój rozmiar.
	
	Skrypt nie jest niczym innowacyjnym. Można to jeszcze bardziej ulepszyć - ale projekt w którym go użyłem - już tego nie wymagał.
	Możliwa modyfikacja to najlepiej pójście w stronę minimalnej konfiguracji - czyli skrypt sam powinien się dostosowywać do sytuacji.
	Tym co w tym prostym mechanizmie jest najlepsze to wcześniej wspomniana dynamika. Efekt pojawiania się w taki sposób informacji
	w grze - nawet tej najmniej ważnej - wygląda nowocześnie i przyjemnie dla oka. Polecam używanie białych oznaczeń czy napisów.
	*/
	
	// publiczna zmienna tekstury nakładanej na obiekt.
    public Texture TextureToIkonerMaterial;
	// Prędkość powiększania się obiektu z informacją.
    public float SpeedMaximizeIkoner = 10.0f;
	// Prędkość zmniejszania się obiektu z informacją.
    public float SpeedMinimalizeIkoner = 15.0f;
	// Zmienna dzięki której można kontrolować czy skryp ma tworzyć obiekt "samodzielnie" czy też korzystać z wcześniej utworzonego
    public int IndexIkonerObj = -1;
	// Domyślna pozycja pojawiania się obiektu
    public Vector3 PosIkoner = new Vector3(3.65f, 0.65f, 0.55f);
	// Zmienna przechowująca prefab Ikonera.
    public GameObject prefabIkonerWindow;

    private Transform IkonerTransform;
    private Material MatIkoner;
    private Texture TexMatIkoner;
    
    // Use this for initialization
    void Start ()
    {
		// Na starcie decyzja czy mamy tworzyć obiekt czy też korzystać z wcześniej stworzeonego. 
        if( IndexIkonerObj == -1 )
        {
			// Funkcja inicjująca tworzenie obiektu zwraca Transform
            IkonerTransform = Init_IkonerObj();
			// Cube staje się dzieckiem GameObjectu na którym jest skrypt
            IkonerTransform.parent = gameObject.transform;
			// Pobieramy index Cuba (czyli dziecka) w hierarchi. Po to aby wiedzieć do czego się odwoływać.
            IndexIkonerObj = IkonerTransform.GetSiblingIndex();
			// Pobieramy komponenty do zmiennych prywatnych przechowujących komponenty
            MatIkoner = IkonerTransform.GetComponent<MeshRenderer>().material;
            MatIkoner.mainTexture = TextureToIkonerMaterial;
            TexMatIkoner = MatIkoner.mainTexture;
			// Obiekt z informacją staje się nieaktywny
            IkonerTransform.gameObject.SetActive(false);

        }
        else if( IndexIkonerObj >= 0 )
        {
			// Pobieramy transform wcześniej utworzonego obiektu przeszukując hierarchię za pomocą indexu który jest przechowywany w zmiennej IndexIkonerObj
            IkonerTransform = gameObject.transform.GetChild(IndexIkonerObj);
			// Pobieramy komponenty do zmiennych prywatnych przechowujących komponenty
            MatIkoner = IkonerTransform.GetComponent<MeshRenderer>().material;
            MatIkoner.mainTexture = TextureToIkonerMaterial;
            TexMatIkoner = MatIkoner.mainTexture;
			// Obiekt z informacją staje się nieaktywny
            IkonerTransform.gameObject.SetActive(false);
        }
    }
	
	// Funkcja na początku projektu w którym została wykorzystana - sama ustalałą kąt i pozycję Cuba z informacją.
	// Zrezygnowałem z tego ponieważ mechanizm nie działał dobrze ze względu na dynamiczne ruchy kamery. Po prostu nie zdawało to ezgaminu.
    Transform Init_IkonerObj()
    {
		// Zamiana rotacji kamery wyrażonej w Vector3 na Quaternion
        Quaternion RotationIkoner = Quaternion.Inverse( Camera.main.transform.rotation );
        //Vector3 tempQuatRot = RotationIkoner.eulerAngles * -1.0f;
        //RotationIkoner = Quaternion.
        //GameObject tempIkonerInstantiate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //tempIkonerInstantiate.transform.localScale = new Vector3(0.0f, 0.0f, 0.1f);
        //tempIkonerInstantiate.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Unlit/Transparent"));
		
		// Utworzenie obiektu do zmiennej która jest lokalna i tymczasowa.
		// Zamiast używać takie rzutowanie można zroibić to za pomocą as:
		// GameObject tempIkonerObj = Instantiate(prefabIkonerWindow, PosIkoner, RotationIkoner, gameObject.transform) as GameObject;
        GameObject tempIkonerObj = (GameObject)Instantiate(prefabIkonerWindow, PosIkoner, RotationIkoner, gameObject.transform);
		// Ponowne przypisanie odpowiednich wartość - wymagał tego projekt. Można te linie usunąć.
        tempIkonerObj.transform.position = PosIkoner;
        tempIkonerObj.transform.rotation = RotationIkoner;
        tempIkonerObj.transform.parent = gameObject.transform;
		// Jako iż obiekt zyskuje rodzica później - należy nadać mu już lokalnąPozycją aby jej użył gdy tego rodzica zyska.
        tempIkonerObj.transform.localPosition = gameObject.transform.InverseTransformDirection(PosIkoner);

        return tempIkonerObj.transform;
    }
	
	// Funkcja zwracająca teksturę (czyli informację) obiektu z informacją. Wymagał tego projekt. 
    public Texture GetTextureMaterialIkoner()
    {
        return TexMatIkoner;
    }

	// Główna funkcja nie IEnumerator - włączająca Corutine - która inicjalizuje pojawienie się Cuba z informacją w świecie gry.
	// To z niej korzysta się najcześciej - tak jak z funkcji wyłączającej informację. Ważny element ideii API tego skryptu.
	// Uruchamia funkcję która również jest "normalna" (nie IEnumerator) - w której dopiero jest uruchamiana Corutyna. 
	// To dlatego, że tutaj następuje włączenie obiektu.
    public void enableShowIkoner()
    {
        IkonerTransform.gameObject.SetActive(true);
        Ikoner_ScaleMaximize();
    }
	
	// Druga główna funkcja API - wyłączająca informację działająca na zasadzie funkcji wyżej.
    public void disableShowIkoner()
    {
        Ikoner_ScaleMinimalize();
    }

	// Funkcja maksymalizująca, bezpośrednio korzystająca z Corutyny - tutaj nie ustawiam "aktywności" obiektu
    public void Ikoner_ScaleMaximize()
    {
        StopCoroutine("IE_IkonerScaler");
        StartCoroutine(IE_IkonerScaler(SpeedMaximizeIkoner, true));
    }
	
	// Funkcja minimalizująca, bezpośrednio korzystająca z Corutyny - tutaj nie ustawiam "aktywności" obiektu
    public void Ikoner_ScaleMinimalize()
    {
        StopCoroutine("IE_IkonerScaler");
        StartCoroutine(IE_IkonerScaler(SpeedMinimalizeIkoner, false));
    }
	
	// Główna funkcja odpowiedzialna za działanie całego mechanizmu.
	// Dzieli się na dwie części - w której sprawdzamy czy mamy maksymalizować czy minimalizować.
	// Jednym z założeń miałabyć też "decyzyjność" skryptu. Czyli miał dochodzić pewnych rzeczy sam.
	// Wyszło to w minimalnym stopniu ale również w tej funkcji dało się to zaimplementować.
	// Otóż mamy JEDNĄ funkcję w której prostym ifem decydujemy co zrobić.
	// Można byłoby rozdzielić to na dwie funkcje - ale po co? Skoro projekt tego nie wymagał (dostępu do każdej z osobna)
	// Funkcja przyjmuje dwa elementy - które możemy konfigurować - prędkość zanikania bądź pojawiania się (speed)
	// Oraz czy ma się maksymalizować czy minimalizować.
    IEnumerator IE_IkonerScaler(float Speed, bool Maximalize)
    {
		// Tutaj tworzymy tymczasowoą zmienną która przechowa aktualną skalę. Tylko tak możemy ją edytować.
        Vector3 tempScale = IkonerTransform.localScale;

        if (Maximalize)
        {
			// Cyfra 2 w warunku pętli wzięła się na potrzeby projektu. Rozmar jako x == 2 wychodził najładniej wizualnie
            while (tempScale.x < 2)
            {
				// Powiększamy obiekt dodając warość Speed w jednostce czasu
				// Bardzo ważna sprawa - zmieniać możemy nasz wektor który przechowuje tymczasową skalę
                tempScale.x += Speed * Time.deltaTime;
                tempScale.y += Speed * Time.deltaTime;
				// Taki wektor - po zmianach - dopiero możemy przypisać obiektowi. Manimpulacja skalą z IkonerTransform.localScale nie jest możliwa
                IkonerTransform.localScale = tempScale;
				// Zwracamy IEnumerator
                yield return 0;
            }
			// Tutaj "wyrównanie" do skali 2. Wymgał tego projekt i zamieszanie z kolizją.
            tempScale.x = 2.0f;
            tempScale.y = 2.0f;
			// Ponowne przypisanie wcześniej wprowadzonych zmian
            IkonerTransform.localScale = tempScale;
        }
        else
        {
			// Wartość w warunku czyli 0.2 powstał z powodu zgłaszanych błędów
			// Miejscami skala osiągała wartość ujemną przez odejmowanie kilku jednostek w pętli
			// To powodowało, że obiekt się nie wyświetlał. 
			// Dokładnie nie odkryłem zależności jaka tutaj istniała - ale testy pozwoliły mi na znalezienie odpowiedniej wartości.
            while (tempScale.x >= 0.2) 
            {
				// Zmniejszanie obiektu
                tempScale.x -= Speed * Time.deltaTime;
                tempScale.y -= Speed * Time.deltaTime;
				// Przypisanie zmian
                IkonerTransform.localScale = tempScale;
				// If odpowiedzialny za wyrównanie skali do zera i pierwsze z zabezpieczeń aby skala nie zeszła na wartość ujemną
                if (tempScale.x > 0.2)
                {
                    tempScale.x = 0.0f;
                    tempScale.y = 0.0f;
                    IkonerTransform.localScale = tempScale;
                }
                yield return 0;
            }
			// Drugie wyrówanie wartości - również dla bezpieczeństwa. Być może nie jest to wydajne - ale działało, a projekt na tym zyskał.
            tempScale.x = 0.0f;
            tempScale.y = 0.0f;
			// Zapisanie zmian
            IkonerTransform.localScale = tempScale;
			// Obiekt ma nie tylko osiągnąć zerową skalę ale również też zniknąć. Po dokonaniu wszystkich operacji - staje się nieaktywny
            IkonerTransform.gameObject.SetActive(false);
        }
    }
}
