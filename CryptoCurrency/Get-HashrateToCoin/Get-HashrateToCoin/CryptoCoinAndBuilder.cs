﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using static ATAP.CryptoCurrency.ExtensionHelpers;
using ATAP.DateTimeUtilities;
namespace ATAP.CryptoCurrency
{
    public enum Proofs
    {
        //[LocalizedDescription("Work", typeof(Resource))]
        [Description("Work")]
        Work,
        [Description("Stake")]
        Stake,
        [Description("Burn")]
        Burn,
        [Description("Activity")]
        Activity,
        [Description("Capacity")]
        Capacity
    }
    // ToDo: automate the creation of the Algorithm enumeration and its attributes based on ???
    // ToDo: it will require the DLL version created to be part of versioning
    // RoDo: it require any changes be integrated into version control.
    public enum Algorithms
    {
        [Description("Casper")]
        Casper,
        [Description("CryptoNote")]
        CryptoNote,
        [Description("Hashcash")]
        Hashcash,
        [Description("Lyra2RE")]
        Lyra2RE,
        [Description("ZeroCoin")]
        ZeroCoin
    }
    // ToDo: automate the creation of the enumeration and its attributes based on the data stored in the GIT project https://github.com/crypti/cryptocurrencies.git. Also look at https://stackoverflow.com/questions/725043/dynamic-enum-in-c-sharp for making the list dynamic
    // ToDo: it will require the DLL version created to be part of versioning
    // RoDo: it require any changes be integrated into version control.
    // There are over 1500+ different documented coins so far
    public enum CoinsE
    {
        [Symbol("BCN")]
        [Description("Bytecoin")]
        [Proof("Work")]
        [Algorithm("CryptoNote")]
        BCN,
        [Proof("Work")]
        [Algorithm("Hashcash")]
        [Symbol("BTC")]
        [Description("BitCoin")]
        BTC,
        [Symbol("ETH")]
        [Description("Ethereum")]
        [Proof("Stake")]
        [Algorithm("Casper")]
        ETH,
        [Symbol("DSH")]
        [Description("Dashcoin ")]
        [Proof("Work")]
        [Algorithm("CryptoNote")]
        DSH,
        [Symbol("XMR")]
        [Description("Monero")]
        [Proof("Work")]
        [Algorithm("CryptoNote")]
        XMR,
        [Algorithm("ZeroCoin")]
        [Symbol("ZEC")]
        [Description("ZCoin")]
        ZEC
    }
    public class BlockReward
    {
        double blockRewardPerBlock;
        public BlockReward(double blockRewardPerBlock)
        {
            this.blockRewardPerBlock = blockRewardPerBlock;
        }
        public double BlockRewardPerBlock { get { return blockRewardPerBlock; } set { blockRewardPerBlock = value; } }
    }
    public partial class CryptoCoinNetworkInfo : ICryptoCoinNetworkInfo
    {
        TimeSpan avgBlockTime;
        double blockRewardPerBlock;
        CoinsE coin;
        DTSandSpan dTSandSpan;
        HashRate hashRate;
        public CryptoCoinNetworkInfo(CoinsE coin)
        {
            this.coin = coin;
        }
        public CryptoCoinNetworkInfo(CoinsE coin, DTSandSpan dtss, HashRate hashRate, TimeSpan avgBlockTime, double blockRewardPerBlock)
        {
            this.coin = coin;
            dTSandSpan = dtss;
            this.hashRate = hashRate;
            this.avgBlockTime = avgBlockTime;
            this.blockRewardPerBlock = blockRewardPerBlock;
        }
        public TimeSpan AvgBlockTime { get => avgBlockTime; set => avgBlockTime = value; }
        public double BlockRewardPerBlock { get { return blockRewardPerBlock; } set { blockRewardPerBlock = value; } }
        public CoinsE Coin
        {
            get { return coin; }
            set { coin = value; }
        }
        public DTSandSpan DTSandSpan { get => dTSandSpan; set => dTSandSpan = value; }
        public HashRate HashRate { get => hashRate; set => hashRate = value; }

        public static double AverageShareOfBlockRewardPerSpanFast(AverageShareOfBlockRewardDT data, TimeSpan timeSpan)
        {
            // normalize into minerHashRateAsAPercentOfTotal the MinerHashRate / NetworkHashRate using the TimeSpan of the Miner
            HashRate minerHashRateAsAPercentOfTotal = data.MinerHashRate / data.NetworkHashRate;
            // normalize the BlockRewardPerSpan to the same span the Miner HashRate span
            //ToDo Fix this calculation
            // normalize the BlockRewardPerSpan to the same span the network HashRate span
            double normalizedBlockCreationSpan = data.AverageBlockCreationSpan.TotalMilliseconds / data.NetworkHashRate.HashRateTimeSpan.TotalMilliseconds;
            double normalizedBlockRewardPerSpan = data.BlockRewardPerBlock / (data.AverageBlockCreationSpan.TotalMilliseconds * normalizedBlockCreationSpan);
            // The number of block rewards found, on average, within a given TimeSpan, is number of blocks in the span, times the fraction of the NetworkHashRate contributed by the miner
            return normalizedBlockRewardPerSpan * (minerHashRateAsAPercentOfTotal.HashRatePerTimeSpan / data.NetworkHashRate.HashRatePerTimeSpan);

        }
        public static double AverageShareOfBlockRewardPerSpanSafe(AverageShareOfBlockRewardDT data, TimeSpan timeSpan)
        {
            // ToDo: Add parameter checking
            return AverageShareOfBlockRewardPerSpanFast(data, timeSpan);
        }

    }

    public interface ICryptoCoinNetworkInfoBuilder
    {
        CryptoCoinNetworkInfo Build();
    }

    public class CryptoCoinNetworkInfoBuilder
    {
        TimeSpan avgBlockTime;
        double blockRewardPerBlock;
        CoinsE coin;
        DTSandSpan dTSandSpan;
        HashRate hashRate;
        public CryptoCoinNetworkInfoBuilder() { }
        public static CryptoCoinNetworkInfoBuilder CreateNew()
        {
            return new CryptoCoinNetworkInfoBuilder();
        }
        public CryptoCoinNetworkInfoBuilder AddAvgBlockTime(TimeSpan avgBlockTime)
        {
            this.avgBlockTime = avgBlockTime;
            return this;
        }
        public CryptoCoinNetworkInfoBuilder AddBlockReward(double blockRewardPerBlock)
        {
            this.blockRewardPerBlock = blockRewardPerBlock;
            return this;
        }
        public CryptoCoinNetworkInfoBuilder AddCoin(CoinsE coin)
        {
            this.coin = coin;
            return this;
        }
        public CryptoCoinNetworkInfoBuilder AddDTSAndSpan(DTSandSpan dTSandSpan)
        {
            this.dTSandSpan = dTSandSpan;
            return this;
        }
        public CryptoCoinNetworkInfoBuilder AddHashRate(HashRate hashRate)
        {
            this.hashRate = hashRate;
            return this;
        }
        public CryptoCoinNetworkInfo Build()
        {
            return new CryptoCoinNetworkInfo(coin, dTSandSpan, hashRate, avgBlockTime, blockRewardPerBlock);
        }
    }
    //public class CryptoCoins
    //{
    //
    //            //Coinname = coinname ?? throw new ArgumentNullException(nameof(coinname));
    //            //Avgblocktime = avgblocktime == TimeSpan.Zero ? throw new ArgumentOutOfRangeException(nameof(avgblocktime)) : avgblocktime;
    //            //Networkhashrate = networkhashrate == Decimal.Zero ? throw new ArgumentOutOfRangeException(nameof(networkhashrate)) : networkhashrate;
    //            //Blockreward = blockreward == Decimal.Zero ? throw new ArgumentOutOfRangeException(nameof(blockreward)) : blockreward;
    //            //Hashrate = hashrate == Decimal.Zero ? throw new ArgumentOutOfRangeException(nameof(hashrate)) : hashrate;
    //        // provides an estimate of the probability


    //    }
    //public class CryptoCoinDifficulty
    //{
    //    string coinname;
    //    string aPIfull;
    //    public string Coinname { get => coinname; set => coinname = value; }
    //    public string APIfull { get => aPIfull; set => aPIfull = value; }
    //    public CryptoCoinDifficulty(string coinname, string aPIfull)
    //    {
    //        Coinname = coinname ?? throw new ArgumentNullException(nameof(coinname));
    //        APIfull = aPIfull ?? throw new ArgumentNullException(nameof(aPIfull));
    //    }
    //    public static async Task<int> GetDifficultyFromAPI(string requestUri)
    //    {
    //        if (string.IsNullOrWhiteSpace(requestUri))
    //        {
    //            throw new ArgumentException("message", nameof(requestUri));
    //        }
    //        // TODO: add validation tests on the requestUri string to ensure it passes basic URI rules
    //        var response = await HttpRequestFactory.Get(requestUri);
    //        // TODO: throw appropriate exception if a bad response is received
    //        // ToDo: parse response based on collection of requestUri rules
    //        // This is for XMR stats
    //        var rstr = response.ContentAsJson();
    //        // parse the JSON
    //        XMRMoneroblocksCoinStats stats = JsonConvert.DeserializeObject<XMRMoneroblocksCoinStats>(response.ContentAsJson());
    //        int dif = stats.difficulty;
    //        return dif;
    //    }
    //}
}
