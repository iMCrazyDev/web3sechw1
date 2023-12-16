const fs = require('fs');
require('dotenv').config();
const { Web3 } = require("web3");

// initialize 
const abi = './abi';
const contractABI = JSON.parse(fs.readFileSync(abi, 'utf8'));
const contractAddress = '0x0677eae7f324ba5a335d085585589c47627e16f7';

// https://docs.infura.io/tutorials/ethereum/send-a-transaction/use-web3.js?q=filter
const network = process.env.ETHEREUM_NETWORK;
const web3 = new Web3(
    new Web3.providers.HttpProvider(
        `https://${network}.infura.io/v3/${process.env.INFURA_API_KEY}`,
    ),
);

const wallet = web3.eth.accounts.wallet.create(0);
web3.eth.accounts.wallet.add(process.env.SIGNER_PRIVATE_KEY);
// console.log(wallet[0].address);
const contract = new web3.eth.Contract(contractABI, contractAddress);
const key = '0x6173647361000000000000000000000000000000000000000000000000000000';
const age = 21;
const name = 'Arsen Andrian Jr.';
const address = '0x77d7bd8e73e0f1cd424acb6b1655d5817f8cebeb';


async function call(key, age, name, address) {
    try {
        const result = await contract.methods.addStruct(key, age, name, address).send({
            from: wallet[0].address,
            gas: 223720,
        });
        console.log('result:', result);
    } catch (error) {
        console.error('exception:', error);
    }
}

// https://ethereum.stackexchange.com/questions/2024/how-to-access-the-event-log-by-knowing-the-contract-address-web3
const filter = {
    fromBlock: 0,
    toBlock: 'latest',
    address: contractAddress,
    topics: [],
};

async function getEvents() {
    try {
        const events = await contract.getPastEvents('allEvents', filter);
        console.log('result:', events);
    } catch (error) {
        console.error('exception:', error);
    }
}

async function getStorageAt(id) {
    try {
        const result = await web3.eth.getStorageAt(contractAddress, id);
        // const [abc, intValue, stringValue, addressValue] = web3.eth.abi.decodeParameters(['bytes32', 'uint256', 'string', 'address'], result);
        // console.log(abc);
        console.log('result:', result);
    } catch (error) {
        console.error('exception:', error);
    }
}

// call(key, age, name, address);
// getEvents();
getStorageAt(0);  // декодировать в структуру у меня это не получилось
