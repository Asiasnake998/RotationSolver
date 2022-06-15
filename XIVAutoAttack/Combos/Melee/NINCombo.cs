using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;

namespace XIVAutoAttack.Combos.Melee;

internal class NINCombo : CustomComboJob<NINGauge>
{
    internal override uint JobID => 30;
    protected override bool ShouldSayout => true;

    public class NinAction : BaseAction
    {
        internal BaseAction[] Ninjutsus { get; }
        public NinAction(uint actionID, params BaseAction[] ninjutsus)
            : base(actionID, false, false)
        {
            Ninjutsus = ninjutsus;
        }
    }

    private static bool _break = false;
    internal static NinAction _ninactionAim = null;

    internal struct Actions
    {
        public static readonly BaseAction

            //隐遁
            Hide = new BaseAction(2245),

            //双刃旋
            SpinningEdge = new BaseAction(2240),

            //残影
            ShadeShift = new BaseAction(2241),

            //绝风
            GustSlash = new BaseAction(2242),

            //飞刀
            ThrowingDagger = new BaseAction(2247),

            //夺取
            Mug = new BaseAction(2248)
            {
                OtherCheck = b => JobGauge.Ninki <= 50,
            },

            //攻其不备
            TrickAttack = new BaseAction(2258)
            {
                BuffsNeed = new ushort[] { ObjectStatus.Suiton, ObjectStatus.Hidden },
                EnermyLocation = EnemyLocation.Back,
            },

            //旋风刃
            AeolianEdge = new BaseAction(2255)
            {
                EnermyLocation = EnemyLocation.Back,
            },

            //血雨飞花
            DeathBlossom = new BaseAction(2254),

            //天之印
            Ten = new BaseAction(2259),

            //地之印
            Chi = new BaseAction(2261),

            //人之印
            Jin = new BaseAction(2263),

            //天地人
            TenChiJin = new BaseAction(7403)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Kassatsu, ObjectStatus.TenChiJin },
            },

            //缩地
            Shukuchi = new BaseAction(2262, true),

            //断绝
            Assassinate = new BaseAction(2246),

            //命水
            Meisui = new BaseAction(16489)
            {
                BuffsNeed = new ushort[] { ObjectStatus.Suiton },
                OtherCheck = b => JobGauge.Ninki < 50
            },

            //生杀予夺
            Kassatsu = new BaseAction(2264)
            {
                OtherCheck = b => Ten.IsCoolDown,
                BuffsProvide = new ushort[] { ObjectStatus.Kassatsu, ObjectStatus.TenChiJin },
            },

            //八卦无刃杀
            HakkeMujinsatsu = new BaseAction(16488),

            //强甲破点突
            ArmorCrush = new BaseAction(3563)
            {
                EnermyLocation = EnemyLocation.Side,
                OtherCheck = b => JobGauge.HutonTimer < 30000 && JobGauge.HutonTimer > 0,
            },

            //通灵之术·大虾蟆
            HellfrogMedium = new BaseAction(7401),

            //六道轮回
            Bhavacakra = new BaseAction(7402),

            //分身之术
            Bunshin = new BaseAction(16493),

            //残影镰鼬
            PhantomKamaitachi = new BaseAction(25774)
            {
                BuffsNeed = new ushort[] { ObjectStatus.PhantomKamaitachiReady },
            },

            //月影雷兽牙
            FleetingRaiju = new BaseAction(25778)
            {
                BuffsNeed = new ushort[] { ObjectStatus.RaijuReady },
            },

            //月影雷兽爪
            ForkedRaiju = new BaseAction(25777)
            {
                BuffsNeed = new ushort[] { ObjectStatus.RaijuReady },
            },

            //风来刃
            Huraijin = new BaseAction(25876)
            {
                OtherCheck = b => JobGauge.HutonTimer == 0,
            },

            //梦幻三段
            DreamWithinaDream = new BaseAction(3566),

            //风魔手里剑天
            FumaShurikenTen = new BaseAction(18873),

            //风魔手里剑人
            FumaShurikenJin = new BaseAction(18875),

            //火遁之术天
            KatonTen = new BaseAction(18876),

            //雷遁之术地
            RaitonChi = new BaseAction(18877),

            //土遁之术地
            DotonChi = new BaseAction(18880),

            //水遁之术人
            SuitonJin = new BaseAction(18881);
        public static readonly NinAction

            //通灵之术
            RabbitMedium = new NinAction(2272),


            //风魔手里剑
            FumaShuriken = new NinAction(2265, Ten)
            {
                AfterUse = ClearNinjutsus,
            },

            //火遁之术
            Katon = new NinAction(2266, Chi, Ten)
            {
                AfterUse = ClearNinjutsus,
            },

            //雷遁之术
            Raiton = new NinAction(2267, Ten, Chi)
            {
                AfterUse = ClearNinjutsus,
            },


            //冰遁之术
            Hyoton = new NinAction(2268, Ten, Jin),

            //风遁之术
            Huton = new NinAction(2269, Jin, Chi, Ten)
            {
                OtherCheck = b => JobGauge.HutonTimer == 0,
                AfterUse = ClearNinjutsus,
            },

            //土遁之术
            Doton = new NinAction(2270, Jin, Ten, Chi)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Doton },
                AfterUse = ClearNinjutsus,
            },

            //水遁之术
            Suiton = new NinAction(2271, Ten, Chi, Jin)
            {
                EnermyLocation = EnemyLocation.Back,
                BuffsProvide = new ushort[] { ObjectStatus.Suiton },
                AfterUse = () =>
                {
                    ClearNinjutsus();
                    _break = false;
                },
            },


            //劫火灭却之术
            GokaMekkyaku = new NinAction(16491, Chi, Ten)
            {
                AfterUse = ClearNinjutsus,
            },


            //冰晶乱流之术
            HyoshoRanryu = new NinAction(16492, Ten, Jin)
            {
                AfterUse = ClearNinjutsus,
            };
    }
    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.单体防御, $"{Actions.ShadeShift.Action.Name}"},
        {DescType.移动, $"{Actions.Shukuchi.Action.Name}，目标为面向夹角小于30°内最远目标。"},
    };
    private static void ChoiceNinjutsus()
    {
        if (Service.IconReplacer.OriginalHook(2260) != 2260) return;
        //有生杀予夺
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Kassatsu))
        {
            if (Actions.GokaMekkyaku.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.GokaMekkyaku;
                return;
            }
            if (Actions.HyoshoRanryu.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.HyoshoRanryu;
                return;
            }

            if (Actions.Katon.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.Katon;
                return;
            }

            if (Actions.Raiton.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.Raiton;
                return;
            }
        }
        else
        {
            //看看能否背刺
            if (Actions.Ten.ShouldUseAction(out _, mustUse: true) && Actions.TrickAttack.RecastTimeRemain < 2 && _break)
            {
                if (Actions.Suiton.ShouldUseAction(out _)) _ninactionAim = Actions.Suiton;
            }
            //常规忍术
            else if (Actions.Ten.ShouldUseAction(out _))
            {
                if (!IsMoving && Actions.Doton.ShouldUseAction(out _))
                {
                    _ninactionAim = Actions.Doton;
                    return;
                }

                if (Actions.Katon.ShouldUseAction(out _))
                {
                    _ninactionAim = Actions.Katon;
                    return;
                }

                if (Actions.Raiton.ShouldUseAction(out _))
                {
                    _ninactionAim = Actions.Raiton;
                    return;
                }

                if (Actions.FumaShuriken.ShouldUseAction(out _))
                {
                    _ninactionAim = Actions.FumaShuriken;
                    return;
                }


                if (!TargetHelper.InBattle)
                {
                    //加状态
                    if (Actions.Huton.ShouldUseAction(out _)) _ninactionAim = Actions.Huton;
                }
            }
        }
    }

    private static void ClearNinjutsus()
    {
        _ninactionAim = null;
    }

    private static bool DoNinjutsus(out IAction act)
    {
        act = null;

        //有天地人
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.TenChiJin))
        {
            uint tenId = Service.IconReplacer.OriginalHook(Actions.Ten.ID);
            uint chiId = Service.IconReplacer.OriginalHook(Actions.Chi.ID);
            uint jinId = Service.IconReplacer.OriginalHook(Actions.Jin.ID);

            //第一个
            if (tenId == Actions.FumaShurikenTen.ID)
            {
                //AOE
                if (Actions.Katon.ShouldUseAction(out _))
                {
                    if (Actions.FumaShurikenJin.ShouldUseAction(out act)) return true;
                }
                //Single
                if (Actions.FumaShurikenTen.ShouldUseAction(out act)) return true;
            }

            //第二击杀AOE
            else if (tenId == Actions.KatonTen.ID)
            {
                if (Actions.KatonTen.ShouldUseAction(out act, mustUse: true)) return true;
            }
            //其他几击
            else if (chiId == Actions.RaitonChi.ID)
            {
                if (Actions.RaitonChi.ShouldUseAction(out act, mustUse: true)) return true;
            }
            else if (chiId == Actions.DotonChi.ID)
            {
                if (Actions.DotonChi.ShouldUseAction(out act, mustUse: true)) return true;
            }
            else if (jinId == Actions.SuitonJin.ID)
            {
                if (Actions.SuitonJin.ShouldUseAction(out act, mustUse: true)) return true;
            }
        }

        if (_ninactionAim == null) return false;

        uint id = Service.IconReplacer.OriginalHook(2260);

        //没开始，释放第一个
        if (id == 2260)
        {
            act = _ninactionAim.Ninjutsus[0];
            return true;
        }
        //失败了
        else if (id == Actions.RabbitMedium.ID)
        {
            act = Actions.RabbitMedium;
            return true;
        }
        //结束了
        else if (id == _ninactionAim.ID)
        {
            if (_ninactionAim.ShouldUseAction(out act, mustUse: true)) return true;
        }
        //释放第二个
        else if (id == Actions.FumaShuriken.ID)
        {
            if (_ninactionAim.Ninjutsus.Length > 1)
            {
                act = _ninactionAim.Ninjutsus[1];
                return true;
            }
        }
        //释放第三个
        else if (id == Actions.Katon.ID || id == Actions.Raiton.ID || id == Actions.Hyoton.ID)
        {
            if (_ninactionAim.Ninjutsus.Length > 2)
            {
                act = _ninactionAim.Ninjutsus[2];
                return true;
            }
        }
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        _break = true;
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (!TargetHelper.InBattle && HaveTargetAngle && Actions.Ten.IsCoolDown && Actions.Hide.ShouldUseAction(out act)) return true;

        ChoiceNinjutsus();
        if (DoNinjutsus(out act)) return true;


        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Ninjutsu))
        {
            //大招
            if (Actions.FleetingRaiju.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.ForkedRaiju.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.PhantomKamaitachi.ShouldUseAction(out act, lastComboActionID)) return true;

            //加状态
            if (Actions.Huraijin.ShouldUseAction(out act, lastComboActionID)) return true;

            //AOE
            if (Actions.HakkeMujinsatsu.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.DeathBlossom.ShouldUseAction(out act, lastComboActionID)) return true;

            //Single
            if (Actions.ArmorCrush.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.AeolianEdge.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.GustSlash.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.SpinningEdge.ShouldUseAction(out act, lastComboActionID)) return true;

            //飞刀
            if (IconReplacer.Move && MoveAbility(1, out act)) return true;
            if (Actions.ThrowingDagger.ShouldUseAction(out act)) return true;
        }
        else
        {
            ClearNinjutsus();
        }

        act = null;
        return false;
    }



    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ShadeShift.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {

        if (Actions.TrickAttack.RecastTimeElapsed <= 12 && Actions.TrickAttack.IsCoolDown)
        {
            if (Actions.TenChiJin.ShouldUseAction(out act)) return true;
            if (Actions.Kassatsu.ShouldUseAction(out act)) return true;
        }
        if (Actions.TrickAttack.ShouldUseAction(out act)) return true;

        if (JobGauge.Ninki >= 50)
        {
            if (Actions.Bunshin.ShouldUseAction(out act)) return true;
            if (Actions.HellfrogMedium.ShouldUseAction(out act)) return true;
            if (Actions.Bhavacakra.ShouldUseAction(out act)) return true;
        }
        if (Actions.Meisui.ShouldUseAction(out act)) return true;


        if (Actions.Mug.ShouldUseAction(out act)) return true;


        if (Service.ClientState.LocalPlayer.Level < Actions.DreamWithinaDream.Level)
        {
            if (Actions.Assassinate.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.DreamWithinaDream.ShouldUseAction(out act)) return true;
        }
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Shukuchi.ShouldUseAction(out act)) return true;

        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }
}