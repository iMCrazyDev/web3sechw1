using System;
using System.IO;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.Contracts;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.Extensions;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;

class Program
{
    static async Task Main()
    {
        var abiPath = "./abi";
        var abi = File.ReadAllText(abiPath);
        var contractAddress = "0x0677eae7f324ba5a335d085585589c47627e16f7";

        var infuraApiKey = "73e4cc21b1ff41c387f0629745ebf246";
        var network = "goerli";
        var signerPrivateKey = "0xa66da406b16d2e76294b32aafb826740d96b1f0996c82aa5aaeb7423b656c304";

        Account abc = new Account(signerPrivateKey, Nethereum.Signer.Chain.Goerli);

        var web3 = new Web3(abc, $"https://{network}.infura.io/v3/{infuraApiKey}");
        var contract = web3.Eth.GetContract(abi, contractAddress);

        var key = new byte[1] { 123 };
        UInt32 age = 21u;
        var name = "Arsen Andrian Jr.";
        var address = "0x77d7bd8e73e0f1cd424acb6b1655d5817f8cebeb";

        // все функции проверил - работает
        // await CallContractMethod(contract, abc, key, age, name, address);
        // await GetPastEvents(contract, contractAddress, web3);
        // await GetStorageAt(web3, contractAddress, 0);
    }

    static async Task CallContractMethod(Contract contract, Account wallet, byte[] key, UInt32 age, string name, string address)
    {
        try
        {
            var result = await contract.GetFunction("addStruct")
                .SendTransactionAsync(wallet.Address, new HexBigInteger(223720), null, key, age, name, address);
            Console.WriteLine("Result: " + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
    }

    static async Task GetPastEvents(Contract contract, string contractAddress, Web3 web3)
    {
        try
        {
            var filter = new NewFilterInput()
            {
                FromBlock = new BlockParameter(0),
                ToBlock = new BlockParameter(),
                Address = new string[1] { contractAddress },
                Topics = new string[] { }
            };

            var transferEventHandler = web3.Eth.GetEvent<AllEventsDTO>(contractAddress);
            var logs = await transferEventHandler.GetAllChangesAsync(filter);
            foreach (var logItem in logs)
            {
                Console.WriteLine(
                    $"tx:{logItem.Log.TransactionHash} {logItem.Event.Address} {logItem.Event.Age} {logItem.Event.Name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
    }

    static async Task GetStorageAt(Web3 web3, string contractAddress, int slotNumber)
    {
        try
        {
            var result = await web3.Eth.GetStorageAt.SendRequestAsync(contractAddress, new HexBigInteger(slotNumber));
            Console.WriteLine("Result: " + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
    }
}

[Event("StructAdded")]
public class AllEventsDTO : IEventDTO
{
    [Parameter("bytes32", "key", 1, true)]
    public byte[] Key { get; set; }

    [Parameter("uint256", "age", 2, false)]
    public BigInteger Age { get; set; }

    [Parameter("string", "name", 3, false)]
    public string Name { get; set; }

    [Parameter("address", "address_", 4, false)]
    public string Address { get; set; }
}
