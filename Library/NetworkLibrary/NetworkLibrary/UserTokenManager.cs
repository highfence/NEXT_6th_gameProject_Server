﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace NetworkLibrary
{
    public class UserTokenManager
    {
        // ConcureentDictionary를 사용한다. 그런데 foreach에서 스레드 세이프한지 테스트 하자. 
        // 스택오버플로어에서는 스레드 세이프하다고 한다.

        // 현재 Users에 추가된 유저 수. ConcurrentDictionary의 Count를 사용하면 일일이 계산하므로 따로 변수로 계산해 놓는다.
        Int64 CurrentCount;

        ConcurrentDictionary<Int64, Session> Users = new ConcurrentDictionary<Int64, Session>();

        Timer TimerHeartbeat;
        long HeartbeatDuration;


        public UserTokenManager() { }


        public void StartHeartbeatChecking(uint check_interval_sec, uint allow_duration_sec)
        {
            HeartbeatDuration = allow_duration_sec * 10000000;
            TimerHeartbeat = new Timer(CheckHeartbeat, null, 1000 * check_interval_sec, 1000 * check_interval_sec);
        }


        public void Add(Session user)
        {
            if (Users.TryAdd(user.UniqueId, user))
            {
                Interlocked.Increment(ref CurrentCount);
            }
        }


        public void Remove(Session user)
        {
            var uniqueId = user.UniqueId;

            if (Users.TryRemove(uniqueId, out var temp))
            {
                Interlocked.Decrement(ref CurrentCount);
            }
        }


        public bool IsExist(Session user)
        {
            return Users.ContainsKey(user.UniqueId);
        }


        public Int64 GetTotalCount()
        {
            Interlocked.Read(ref CurrentCount);
            return CurrentCount;
        }


        void CheckHeartbeat(object state)
        {
            long allowed_time = DateTime.Now.Ticks - this.HeartbeatDuration;

            foreach (var user in Users.Values)
            {
                long heartbeat_time = user.LatestHeartbeatTime;
                if (heartbeat_time >= allowed_time)
                {
                    continue;
                }

                //TODO: 여기서 불러도 스레드 세이프 한지 꼼꼼하게 알아보기
                user.DisConnect();
            }
        }
    }
}
