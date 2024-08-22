
namespace Vote.Monitor.Domain.UnitTests.Entities;
internal class FormTestData
{
    public const string QuestionsJson = """
                [
          {
            "Id": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0",
            "Code": "A",
            "Text": {
              "RO": "Secţia de votare se află în"
            },
            "Options": [
              {
                "Id": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
                "Text": {
                  "RO": "România"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "354a6dad-de83-45c8-93cd-cd60759a26e1",
                "Text": {
                  "RO": "Străinătate"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "chiar dacă aţi mai răspuns la întrebare, este necesară pentru a diferenţia formularele în funcţie de proceduri"
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "e7017ad7-a56a-462b-9f6f-91aa4c6cc4e7",
            "Code": "B1",
            "Text": {
              "RO": "La ora 22:00 mai erau alegători în aşteptare în afara sălii unde se votează\t"
            },
            "Options": [
              {
                "Id": "8c0bc581-03a0-4ae7-9ef7-772f383c6068",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cd8afa93-1aff-472a-87ab-cb6c5a617a78",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "dd84cdeb-f272-4fc8-a86d-9fbb2e364dcd",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "a13473db-3e06-49db-919c-1f65df2bebc5",
            "Code": "B1.1",
            "Text": {
              "RO": "Alegătorilor aflaţi în aşteptare la ora 22:00 li s-a permis să voteze"
            },
            "Options": [
              {
                "Id": "bb64f46f-481e-4ee3-9fc0-49b7eee2dbd0",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "d0d5249c-05c5-48a2-b802-35eeb9a1b525",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "44398344-97a5-4988-ad03-c36cbc2eec18",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "8c0bc581-03a0-4ae7-9ef7-772f383c6068",
              "Condition": "Includes",
              "ParentQuestionId": "e7017ad7-a56a-462b-9f6f-91aa4c6cc4e7"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "3a67c953-f379-4bdb-98d8-1c8251e2aa7f",
            "Code": "B2",
            "Text": {
              "RO": "Secţia de votare s-a închis la ora 22:00"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "d9277409-cd67-4927-bc66-d6d29df659f1",
            "Code": "B2.1",
            "Text": {
              "RO": "La ce oră s-a închis secţia de votare"
            },
            "Options": [
              {
                "Id": "ccaea6dc-1229-4ea4-a959-a1e1f827667d",
                "Text": {
                  "RO": "înainte de 22:00"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "8238f08b-df88-4d5d-9140-083053efe5dc",
                "Text": {
                  "RO": "22:00-22:15"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "96e0820b-2c94-4279-a657-b929f52e998d",
                "Text": {
                  "RO": "22:15-22:30"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e43f8987-804c-4990-9c49-de824ad166f7",
                "Text": {
                  "RO": "22:30-23:00"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "205aa170-24a9-4909-b55e-2e11832963c6",
                "Text": {
                  "RO": "după 23:59"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "e859a899-8c5a-45c8-931b-d78745cd769d",
              "Condition": "Includes",
              "ParentQuestionId": "744b46dd-74b2-41cc-8848-541d5b56c1e8"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "5ab37b4d-de16-4f83-be6e-685d84a28336",
            "Code": "C1",
            "Text": {
              "RO": "BESV are numărul minim de 5 membri (3 în străinătate)  prezenţi"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "inclusiv președinte și locțiitor"
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "64d5251a-2048-4dfc-b4e0-fcffae2c68bd",
            "Code": "C2",
            "Text": {
              "RO": "Care sunt partidele reprezentate în BESV"
            },
            "Options": [
              {
                "Id": "78dec6f7-6ea0-422c-8182-e0b3525f4dd9",
                "Text": {
                  "RO": "PSD"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "2ea69a7d-c59d-4877-9c34-154317b90fc2",
                "Text": {
                  "RO": "PNL"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "bd159fb3-52d5-45cd-a3bb-96ae63e326af",
                "Text": {
                  "RO": "USR"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "fd84105a-4580-4d17-8eac-7cc7b55807ec",
                "Text": {
                  "RO": "UDMR"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "edfcd6e8-79e5-4b2b-b6a0-bf843f6c505c",
                "Text": {
                  "RO": "AUR"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "97b280cb-af91-4783-9f89-8e9e913560b0",
                "Text": {
                  "RO": "Altele (specificaţi)"
                },
                "IsFlagged": false,
                "IsFreeText": true
              }
            ],
            "Helptext": {
              "RO": "chiar dacă aţi răspuns deja în secţiunea descrierii generale a secţiei de votare"
            },
            "DisplayLogic": null,
            "$questionType": "multiSelectQuestion"
          },
          {
            "Id": "c1890e03-6dd2-4bc3-9da4-4d3ae2860dc3",
            "Code": "C3",
            "Text": {
              "RO": "La secţia de votare au fost prezenţi şi alţi observatori"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "chiar dacă aţi răspuns deja în secţiunea descrierii generale a secţiei de votare"
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "6dca9ae4-9028-4fe8-ae6c-eb3ddc3260a6",
            "Code": "C3.1",
            "Text": {
              "RO": "Ce organizaţii au acreditat ceilalţi observatori de la secţia de votare"
            },
            "Options": [
              {
                "Id": "33603265-1f67-4309-82ec-2a87da26d1a8",
                "Text": {
                  "RO": "Millenium for Human Rights "
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "21df06e0-5161-4510-ad56-fe81aca4fcc5",
                "Text": {
                  "RO": "Tineretul pentru Naţiunile Unite"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "d8a4c17d-203d-4c0d-aa40-00c4b610ad31",
                "Text": {
                  "RO": "Altele (specificaţi)"
                },
                "IsFlagged": false,
                "IsFreeText": true
              }
            ],
            "Helptext": {
              "RO": "chiar dacă aţi răspuns deja în secţiunea descrierii generale a secţiei de votare"
            },
            "DisplayLogic": {
              "Value": "86735b49-7dcf-4621-91bf-e0b6467f592b",
              "Condition": "Includes",
              "ParentQuestionId": "c1890e03-6dd2-4bc3-9da4-4d3ae2860dc3"
            },
            "$questionType": "multiSelectQuestion"
          },
          {
            "Id": "d6322efd-a054-4659-bb6b-d125b0925cc8",
            "Code": "C4",
            "Text": {
              "RO": "În secţia de votare s-au aflat persoane neautorizate"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "chiar dacă aţi răspuns deja în secţiunea descrierii generale a secţiei de votare"
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "a03375b2-a8c1-43b9-b99a-b967e2070af9",
            "Code": "C4.1",
            "Text": {
              "RO": "Persoane neautorizate în secţia de votare"
            },
            "Options": [
              {
                "Id": "800cbe6f-cea1-4d4f-92b6-76b3401dc2ea",
                "Text": {
                  "RO": "Autorităţi locale (care nu sunt candidaţi)"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "f9b60fc3-4d9b-407f-b301-666d0c209ff4",
                "Text": {
                  "RO": "Poliţie/pază"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "18204b58-c66a-4a48-bf77-c2e3604ebacc",
                "Text": {
                  "RO": "Altele (specificaţi)"
                },
                "IsFlagged": false,
                "IsFreeText": true
              }
            ],
            "Helptext": {
              "RO": "chiar dacă aţi răspuns deja în secţiunea descrierii generale a secţiei de votare"
            },
            "DisplayLogic": null,
            "$questionType": "multiSelectQuestion"
          },
          {
            "Id": "764688a3-ba5e-47e2-bfb5-6a26400a155a",
            "Code": "C5",
            "Text": {
              "RO": "Persoane din afara BESV au intervenit în proces"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "8ace2362-78b9-4253-89f4-a9edfb37dc2b",
            "Code": "E1",
            "Text": {
              "RO": "Operatorul SIMPV a închis votarea în interfaţa ADV\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "a4f50548-75ed-428e-9a11-ed2d9b95df7c",
            "Code": "E2",
            "Text": {
              "RO": "A fost oprită camera video, iar cardul sigilat"
            },
            "Options": [
              {
                "Id": "6b4a4a4a-9f1b-4af0-b00c-fe3e69f81a7f",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "8d9b5c6c-3bc7-4a70-8279-6d18cb558ab1",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "f75a4d22-8aaa-4102-9f26-de3d80eea7cf",
                "Text": {
                  "RO": "NU ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "354a6dad-de83-45c8-93cd-cd60759a26e1",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "d461c46e-aa9c-439f-8d6d-d2cb7c169b3a",
            "Code": "E3",
            "Text": {
              "RO": "Președintele BESV a verificat starea sigiliilor de pe urne\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "67a69d7a-f218-4371-9350-297b3cd966a6",
            "Code": "E3",
            "Text": {
              "RO": "Președintele BESV a verificat starea sigiliilor de pe urnă"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "prin numărarea tuturor semnăturilor"
            },
            "DisplayLogic": {
              "Value": "354a6dad-de83-45c8-93cd-cd60759a26e1",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "e35e8855-5ece-4491-ad16-ba5e991d3b4c",
            "Code": "E4",
            "Text": {
              "RO": "Președintele BESV a sigilat fanta urnelor\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "2008a9a2-4a77-49fc-9229-fa91650dd91b",
            "Code": "E4",
            "Text": {
              "RO": "Președintele BESV a sigilat fanta urnei"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "354a6dad-de83-45c8-93cd-cd60759a26e1",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "08ba238b-185c-40ce-b80f-310620f971f9",
            "Code": "E5",
            "Text": {
              "RO": "Ștampilele au fost introduse într-un plic și sigilate\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "7bf68a85-2f5c-4eef-8f1c-a5d7415aebf5",
            "Code": "E6",
            "Text": {
              "RO": "Preşedintele BESV a anulat buletinele de vot neîntrebuințate şi a consemnat numărul lor pentru fiecare scrutin \t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "60bc7986-7932-474e-9e94-c5f93ef53f3b",
            "Code": "E6",
            "Text": {
              "RO": "Preşedintele BESV a anulat buletinele de vot neîntrebuințate şi a consemnat numărul lor"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "354a6dad-de83-45c8-93cd-cd60759a26e1",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "0a5ae200-5d14-4164-8848-e103a087880a",
            "Code": "E7",
            "Text": {
              "RO": "Preşedintele BESV a consemnat numărul alegătorilor înscrişi în liste pentru fiecare scrutin\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "3f32556e-05ac-48b4-b9d5-8cfcaab597de",
            "Code": "E7",
            "Text": {
              "RO": "Preşedintele BESV a consemnat numărul alegătorilor înscrişi în liste"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "354a6dad-de83-45c8-93cd-cd60759a26e1",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "574a510f-8553-46bd-96a3-a15fe11043ad",
            "Code": "E8",
            "Text": {
              "RO": "Preşedintele BESV a stabilit și consemnat numărul alegătorilor care au votat pentru fiecare scrutin"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "prin numărarea tuturor semnăturilor"
            },
            "DisplayLogic": {
              "Value": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "26ca552d-7212-4929-b01f-f693a3c4402c",
            "Code": "E8",
            "Text": {
              "RO": "Preşedintele BESV a stabilit și consemnat numărul alegătorilor care au votat"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "prin numărarea tuturor semnăturilor"
            },
            "DisplayLogic": {
              "Value": "354a6dad-de83-45c8-93cd-cd60759a26e1",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "11edeef0-0c9a-43e4-9d58-168748fdd4ea",
            "Code": "F1",
            "Text": {
              "RO": "Sigiliile de pe urnele staţionare erau intacte\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "744b46dd-74b2-41cc-8848-541d5b56c1e8",
            "Code": "F1",
            "Text": {
              "RO": "Sigiliile de pe urnă erau intacte\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "354a6dad-de83-45c8-93cd-cd60759a26e1",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "2260af11-2c14-49f4-9f20-3144b8b3795e",
            "Code": "F2",
            "Text": {
              "RO": "Sigiliile de pe urna specială (mobilă) erau intacte"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "bifaţi doar dacă s-a votat cu urna specială"
            },
            "DisplayLogic": {
              "Value": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "e67455a4-e24e-4e57-a15d-411770c77f53",
            "Code": "F3",
            "Text": {
              "RO": "Toţi membrii BESV au avut posibilitatea de a examina buletinele de vot\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "62d8222b-2dd0-4837-b15c-b7fc3e9a568d",
            "Code": "F4",
            "Text": {
              "RO": "BESV a supus la vot buletinele de vot contestate "
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "bifaţi numai dacă au existat voturi contestate"
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "a6755e40-8efa-4276-9f7e-a4ba7790893f",
            "Code": "F5",
            "Text": {
              "RO": "Validitatea voturilor a fost stabilită după criterii rezonabile şi aplicate în mod consecvent \t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "8b9adcd2-673f-4738-bd36-ec88a1e10014",
            "Code": "F6",
            "Text": {
              "RO": "Ordinea etapelor numărării voturilor a fost strict respectată\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "1efd68ec-cb5d-441d-af0f-9b08f8d180ff",
            "Code": "F7",
            "Text": {
              "RO": "Înregistrarea audio-video cu tableta a decurs neîntrerupt\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "4e5bb5b5-29ef-4517-b792-cdffe4b7792d",
            "Code": "F8",
            "Text": {
              "RO": "Urnele au fost deschise una câte una\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "0b2a16b8-7294-4412-8aac-9b029261eb7e",
            "Code": "F9",
            "Text": {
              "RO": "Buletinele de vot din toate urnele au fost sortate înaintea numărării \t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": {
              "Value": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8",
              "Condition": "Includes",
              "ParentQuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0"
            },
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "3a18a964-7b25-4b5a-9d1d-47086963b6c3",
            "Code": "G1",
            "Text": {
              "RO": "Rezultatele au fost consemnate corect în procesele verbale\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "394f8f8c-7171-4885-967c-b2ce4477ed18",
            "Code": "G2",
            "Text": {
              "RO": "Rezultatele au fost introduse în formularul pentru verificarea corelaţiilor "
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": "(în tableta SIMPV)\t"
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "4232cc65-8309-4297-a8c5-8fca8083324d",
            "Code": "G3",
            "Text": {
              "RO": "Cheile de verificare s-au închis\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "8a22719c-3e33-451f-900e-7cccd55cfa41",
            "Code": "G4",
            "Text": {
              "RO": "Rezultatele au fost transmise prin intermediul tabletei\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "8e913919-349d-4169-8667-cf0408549fbb",
            "Code": "G5",
            "Text": {
              "RO": "Preşedintele BESV a avut dificultăţi în completarea proceselor verbale\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "a0ba98d7-d9ce-42b8-8ecb-fb3cefa610c4",
            "Code": "G6",
            "Text": {
              "RO": "Unul sau mai mulţi membri BESV au refuzat să semneze procesele verbale\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "a5a2a7dd-a53f-4262-95e0-5f61ebc8ce3b",
            "Code": "H1",
            "Text": {
              "RO": "Mai multe buletine de vot de acelaşi fel împăturite împreună în urnă\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "31e3531b-52b4-4acd-ac2d-90e04a2af1cf",
            "Code": "H2",
            "Text": {
              "RO": "Observatori îndepărtaţi din secţia de votare în timpul numărării voturilor\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "52b7a579-21c6-4ce2-945a-378164d51302",
            "Code": "H3",
            "Text": {
              "RO": "Persoane care nu fac parte din BESV participă la numărarea voturilor\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "a564bb15-e059-4955-b8bb-9c9e4f4dedaa",
            "Code": "H4",
            "Text": {
              "RO": "Procesele verbale au fost semnate în alb de membrii BESV\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "f3351342-a9ab-45db-9c00-a81ee88c8b3c",
            "Code": "H5",
            "Text": {
              "RO": "Aţi observat indicii privind falsificarea rezultatelor\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "3395cdb1-c7e2-4845-8498-ada56bfbdeb8",
            "Code": "H6",
            "Text": {
              "RO": "Aţi observat erori sau omisiuni procedurale importante\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "f2caca9c-aae0-4b89-8b08-e2d323c5b524",
            "Code": "H7",
            "Text": {
              "RO": "Aţi observat atmosferă tensionată\t"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cc13841d-dd89-4bbb-9b37-5976914053ae",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "a39a3b41-e047-49bd-8c04-ac6475bff9ea",
            "Code": "H8",
            "Text": {
              "RO": "Alte probleme\t"
            },
            "Options": [
              {
                "Id": "85bb2bd6-fc93-4d3c-b0cd-5aabcc974cca",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "cb979b55-67ba-40b8-8afd-2b67fd203769",
                "Text": {
                  "RO": "DA (specificaţi)"
                },
                "IsFlagged": false,
                "IsFreeText": true
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "28fd2710-b934-486a-9d61-3f9cfa1ba11e",
            "Code": "I1",
            "Text": {
              "RO": "Toţi cei prezenţi, inclusiv echipa dumneavoastră, au putut urmări procedurile fără restricții"
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": true,
                "IsFreeText": false
              },
              {
                "Id": "00227c9f-99d9-4983-a5b9-53276f233291",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "8c8638bb-6d0c-4cb6-9d20-ad724e895a5a",
            "Code": "I2",
            "Text": {
              "RO": "Membrii BESV au fost cooperanţi şi v-au pus la dispoziţie informaţiile cerute "
            },
            "Options": [
              {
                "Id": "86735b49-7dcf-4621-91bf-e0b6467f592b",
                "Text": {
                  "RO": "DA"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "e859a899-8c5a-45c8-931b-d78745cd769d",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "8a639b78-6b49-4101-b5ef-66aa373d9081",
            "Code": "I3",
            "Text": {
              "RO": "Au fost depuse întâmpinări la preşedintele BESV"
            },
            "Options": [
              {
                "Id": "e31d08bd-d7d0-4475-9674-c49c9edff284",
                "Text": {
                  "RO": "NU"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "fcae8dde-11c2-4118-9192-29b9353bcb4c",
                "Text": {
                  "RO": "Nu ştiu"
                },
                "IsFlagged": false,
                "IsFreeText": false
              },
              {
                "Id": "d77030a7-f9b3-4f96-881b-ac8147f42327",
                "Text": {
                  "RO": "DA (specificaţi câte)"
                },
                "IsFlagged": false,
                "IsFreeText": true
              }
            ],
            "Helptext": {
              "RO": ""
            },
            "DisplayLogic": null,
            "$questionType": "singleSelectQuestion"
          },
          {
            "Id": "73675951-fa34-499b-9682-bd243edee5c4",
            "Code": "J1",
            "Text": {
              "RO": "Evaluarea procedurii de numărare a voturilor"
            },
            "Scale": "OneTo4",
            "Helptext": {
              "RO": "1-foarte proastă; 2-proastă; 3-bună; 4-foarte bună"
            },
            "DisplayLogic": null,
            "$questionType": "ratingQuestion"
          },
          {
            "Id": "39ec0b02-30e7-46c3-9764-6ca6d28434c1",
            "Code": "J2.1",
            "Text": {
              "RO": "Situaţia şi atmosfera generală \t"
            },
            "Scale": "OneTo5",
            "Helptext": {
              "RO": "1-foarte proastă; 2-proastă; 3-medie; 4-bună; 5-foarte bună"
            },
            "DisplayLogic": null,
            "$questionType": "ratingQuestion"
          },
          {
            "Id": "c16d4573-3b7d-40e9-8d95-42a276bcb6f1",
            "Code": "J2.2",
            "Text": {
              "RO": "Respectarea procedurilor \t"
            },
            "Scale": "OneTo5",
            "Helptext": {
              "RO": "1-foarte proastă; 2-proastă; 3-medie; 4-bună; 5-foarte bună"
            },
            "DisplayLogic": null,
            "$questionType": "ratingQuestion"
          },
          {
            "Id": "5e6b14d2-edb4-4987-a281-b8700cc5ca18",
            "Code": "J2.3",
            "Text": {
              "RO": "Cât de bine înţeleg membrii BESV procedura de numărare a voturilor\t"
            },
            "Scale": "OneTo5",
            "Helptext": {
              "RO": "1-foarte proastă; 2-proastă; 3-medie; 4-bună; 5-foarte bună"
            },
            "DisplayLogic": null,
            "$questionType": "ratingQuestion"
          },
          {
            "Id": "41d0fac5-98b5-41ed-9846-1820aacdbb0f",
            "Code": "J2.4",
            "Text": {
              "RO": "Cât de bine înţelege preşedintele BESV procedura de consemnare a rezultatelor\t"
            },
            "Scale": "OneTo5",
            "Helptext": {
              "RO": "1-foarte proastă; 2-proastă; 3-medie; 4-bună; 5-foarte bună"
            },
            "DisplayLogic": null,
            "$questionType": "ratingQuestion"
          },
          {
            "Id": "c0f00091-4871-4a90-9c28-032c30b70835",
            "Code": "J2.5",
            "Text": {
              "RO": "Activitatea BESV \t"
            },
            "Scale": "OneTo5",
            "Helptext": {
              "RO": "1-foarte proastă; 2-proastă; 3-medie; 4-bună; 5-foarte bună"
            },
            "DisplayLogic": null,
            "$questionType": "ratingQuestion"
          },
          {
            "Id": "9174b0d0-06c9-4a05-96ad-975b479eb7a7",
            "Code": "J2.6",
            "Text": {
              "RO": "Transparenţa procesului \t"
            },
            "Scale": "OneTo5",
            "Helptext": {
              "RO": "1-foarte proastă; 2-proastă; 3-medie; 4-bună; 5-foarte bună"
            },
            "DisplayLogic": null,
            "$questionType": "ratingQuestion"
          }
        ]
        """; 
    
    public const string AnswersJson = """
                [
          {
            "Selection": {
              "Text": null,
              "OptionId": "ccf2b29c-2d63-4f4e-a63b-ffa2c573bcf8"
            },
            "QuestionId": "8bd11ef3-306f-435d-9bda-988b7ea3f4a0",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "8c0bc581-03a0-4ae7-9ef7-772f383c6068"
            },
            "QuestionId": "e7017ad7-a56a-462b-9f6f-91aa4c6cc4e7",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "d0d5249c-05c5-48a2-b802-35eeb9a1b525"
            },
            "QuestionId": "a13473db-3e06-49db-919c-1f65df2bebc5",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "3a67c953-f379-4bdb-98d8-1c8251e2aa7f",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "5ab37b4d-de16-4f83-be6e-685d84a28336",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": [
              {
                "Text": null,
                "OptionId": "78dec6f7-6ea0-422c-8182-e0b3525f4dd9"
              },
              {
                "Text": null,
                "OptionId": "2ea69a7d-c59d-4877-9c34-154317b90fc2"
              },
              {
                "Text": null,
                "OptionId": "edfcd6e8-79e5-4b2b-b6a0-bf843f6c505c"
              },
              {
                "Text": "LOREM",
                "OptionId": "97b280cb-af91-4783-9f89-8e9e913560b0"
              }
            ],
            "QuestionId": "64d5251a-2048-4dfc-b4e0-fcffae2c68bd",
            "$answerType": "multiSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "c1890e03-6dd2-4bc3-9da4-4d3ae2860dc3",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "d6322efd-a054-4659-bb6b-d125b0925cc8",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "764688a3-ba5e-47e2-bfb5-6a26400a155a",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "8ace2362-78b9-4253-89f4-a9edfb37dc2b",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "d461c46e-aa9c-439f-8d6d-d2cb7c169b3a",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "e35e8855-5ece-4491-ad16-ba5e991d3b4c",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "08ba238b-185c-40ce-b80f-310620f971f9",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "7bf68a85-2f5c-4eef-8f1c-a5d7415aebf5",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "0a5ae200-5d14-4164-8848-e103a087880a",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "574a510f-8553-46bd-96a3-a15fe11043ad",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "11edeef0-0c9a-43e4-9d58-168748fdd4ea",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "2260af11-2c14-49f4-9f20-3144b8b3795e",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "e67455a4-e24e-4e57-a15d-411770c77f53",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "62d8222b-2dd0-4837-b15c-b7fc3e9a568d",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "a6755e40-8efa-4276-9f7e-a4ba7790893f",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "8b9adcd2-673f-4738-bd36-ec88a1e10014",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "1efd68ec-cb5d-441d-af0f-9b08f8d180ff",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "4e5bb5b5-29ef-4517-b792-cdffe4b7792d",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "0b2a16b8-7294-4412-8aac-9b029261eb7e",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "3a18a964-7b25-4b5a-9d1d-47086963b6c3",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "394f8f8c-7171-4885-967c-b2ce4477ed18",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "4232cc65-8309-4297-a8c5-8fca8083324d",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": [
              {
                "Text": "LOREM",
                "OptionId": "18204b58-c66a-4a48-bf77-c2e3604ebacc"
              }
            ],
            "QuestionId": "a03375b2-a8c1-43b9-b99a-b967e2070af9",
            "$answerType": "multiSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "f3351342-a9ab-45db-9c00-a81ee88c8b3c",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "3395cdb1-c7e2-4845-8498-ada56bfbdeb8",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "f2caca9c-aae0-4b89-8b08-e2d323c5b524",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "85bb2bd6-fc93-4d3c-b0cd-5aabcc974cca"
            },
            "QuestionId": "a39a3b41-e047-49bd-8c04-ac6475bff9ea",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "28fd2710-b934-486a-9d61-3f9cfa1ba11e",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "8c8638bb-6d0c-4cb6-9d20-ad724e895a5a",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e31d08bd-d7d0-4475-9674-c49c9edff284"
            },
            "QuestionId": "8a639b78-6b49-4101-b5ef-66aa373d9081",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Value": 4,
            "QuestionId": "73675951-fa34-499b-9682-bd243edee5c4",
            "$answerType": "ratingAnswer"
          },
          {
            "Value": 5,
            "QuestionId": "39ec0b02-30e7-46c3-9764-6ca6d28434c1",
            "$answerType": "ratingAnswer"
          },
          {
            "Value": 5,
            "QuestionId": "c16d4573-3b7d-40e9-8d95-42a276bcb6f1",
            "$answerType": "ratingAnswer"
          },
          {
            "Value": 5,
            "QuestionId": "5e6b14d2-edb4-4987-a281-b8700cc5ca18",
            "$answerType": "ratingAnswer"
          },
          {
            "Value": 5,
            "QuestionId": "41d0fac5-98b5-41ed-9846-1820aacdbb0f",
            "$answerType": "ratingAnswer"
          },
          {
            "Value": 5,
            "QuestionId": "c0f00091-4871-4a90-9c28-032c30b70835",
            "$answerType": "ratingAnswer"
          },
          {
            "Value": 5,
            "QuestionId": "9174b0d0-06c9-4a05-96ad-975b479eb7a7",
            "$answerType": "ratingAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "8e913919-349d-4169-8667-cf0408549fbb",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "a0ba98d7-d9ce-42b8-8ecb-fb3cefa610c4",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "a5a2a7dd-a53f-4262-95e0-5f61ebc8ce3b",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "31e3531b-52b4-4acd-ac2d-90e04a2af1cf",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "52b7a579-21c6-4ce2-945a-378164d51302",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "e859a899-8c5a-45c8-931b-d78745cd769d"
            },
            "QuestionId": "a564bb15-e059-4955-b8bb-9c9e4f4dedaa",
            "$answerType": "singleSelectAnswer"
          },
          {
            "Selection": {
              "Text": null,
              "OptionId": "86735b49-7dcf-4621-91bf-e0b6467f592b"
            },
            "QuestionId": "8a22719c-3e33-451f-900e-7cccd55cfa41",
            "$answerType": "singleSelectAnswer"
          }
        ]
        """;
}
