using System.Collections.Generic;
using Naninovel;

namespace PleaseBeHappyAccess
{
    /// <summary>
    /// The mod's own spoken messages, in every language the game ships
    /// (locale IDs from StreamingAssets/Assets/Resources/AkemiLocalisedAssetsRoot: en, ja, ko, zh-CN, zh-TW).
    /// </summary>
    internal static class Strings
    {
        static readonly Dictionary<string, Dictionary<string, string>> table = new Dictionary<string, Dictionary<string, string>>
        {
            ["loaded"] = new Dictionary<string, string>
            {
                ["en"] = "Please Be Happy Access loaded.",
                ["ja"] = "Please Be Happy アクセシビリティMODを読み込みました。",
                ["ko"] = "Please Be Happy 접근성 모드를 불러왔습니다.",
                ["zh-CN"] = "Please Be Happy 无障碍模组已加载。",
                ["zh-TW"] = "Please Be Happy 無障礙模組已載入。",
            },
            ["voicedOn"] = new Dictionary<string, string>
            {
                ["en"] = "Reading voiced lines: on.",
                ["ja"] = "ボイス付きのセリフの読み上げ：オン。",
                ["ko"] = "음성 대사 읽기: 켜짐.",
                ["zh-CN"] = "朗读有配音的台词：开。",
                ["zh-TW"] = "朗讀有配音的台詞：開。",
            },
            ["voicedOff"] = new Dictionary<string, string>
            {
                ["en"] = "Reading voiced lines: off.",
                ["ja"] = "ボイス付きのセリフの読み上げ：オフ。",
                ["ko"] = "음성 대사 읽기: 꺼짐.",
                ["zh-CN"] = "朗读有配音的台词：关。",
                ["zh-TW"] = "朗讀有配音的台詞：關。",
            },
            ["modSettings"] = new Dictionary<string, string>
            {
                ["en"] = "Mod settings",
                ["ja"] = "MOD設定",
                ["ko"] = "모드 설정",
                ["zh-CN"] = "模组设置",
                ["zh-TW"] = "模組設定",
            },
            ["modSettingsClosed"] = new Dictionary<string, string>
            {
                ["en"] = "Mod settings closed.",
                ["ja"] = "MOD設定を閉じました。",
                ["ko"] = "모드 설정을 닫았습니다.",
                ["zh-CN"] = "已关闭模组设置。",
                ["zh-TW"] = "已關閉模組設定。",
            },
            ["itemOf"] = new Dictionary<string, string>
            {
                ["en"] = "{0} of {1}",
                ["ja"] = "{1}項目中{0}番目",
                ["ko"] = "{1}개 중 {0}번째",
                ["zh-CN"] = "第{0}项，共{1}项",
                ["zh-TW"] = "第{0}項，共{1}項",
            },
            ["settingReadVoiced"] = new Dictionary<string, string>
            {
                ["en"] = "Read voiced lines",
                ["ja"] = "ボイス付きのセリフを読み上げる",
                ["ko"] = "음성 대사 읽기",
                ["zh-CN"] = "朗读有配音的台词",
                ["zh-TW"] = "朗讀有配音的台詞",
            },
            ["settingCheckUpdates"] = new Dictionary<string, string>
            {
                ["en"] = "Check for updates when the game starts",
                ["ja"] = "ゲーム起動時に更新を確認する",
                ["ko"] = "게임 시작 시 업데이트 확인",
                ["zh-CN"] = "游戏启动时检查更新",
                ["zh-TW"] = "遊戲啟動時檢查更新",
            },
            ["updateAvailable"] = new Dictionary<string, string>
            {
                ["en"] = "Please Be Happy Access {0} is available. To update, close the game and run Please Be Happy Access Installer.",
                ["ja"] = "Please Be Happy Access {0} が公開されています。更新するには、ゲームを閉じて Please Be Happy Access Installer を実行してください。",
                ["ko"] = "Please Be Happy Access {0} 버전이 나왔습니다. 업데이트하려면 게임을 닫고 Please Be Happy Access Installer를 실행하세요.",
                ["zh-CN"] = "Please Be Happy Access {0} 已发布。要更新，请关闭游戏并运行 Please Be Happy Access Installer。",
                ["zh-TW"] = "Please Be Happy Access {0} 已發布。要更新，請關閉遊戲並執行 Please Be Happy Access Installer。",
            },
            ["settingDescribeScenes"] = new Dictionary<string, string>
            {
                ["en"] = "Describe backgrounds and illustrations as they appear",
                ["ja"] = "背景とイラストが表示されたら説明する",
                ["ko"] = "배경과 일러스트가 나타나면 설명하기",
                ["zh-CN"] = "背景和插图出现时进行描述",
                ["zh-TW"] = "背景和插圖出現時進行描述",
            },
            ["settingDescribeCharacters"] = new Dictionary<string, string>
            {
                ["en"] = "Describe characters the first time you meet them",
                ["ja"] = "初めて会ったキャラクターの姿を説明する",
                ["ko"] = "처음 만난 캐릭터의 모습 설명하기",
                ["zh-CN"] = "第一次遇到角色时描述其外貌",
                ["zh-TW"] = "第一次遇到角色時描述其外貌",
            },
            ["settingReadPopups"] = new Dictionary<string, string>
            {
                ["en"] = "Read pop-ups: achievements, day and time",
                ["ja"] = "ポップアップを読み上げる（実績、日付と時間帯）",
                ["ko"] = "팝업 읽기: 업적, 날짜와 시간대",
                ["zh-CN"] = "朗读弹出提示：成就、日期和时段",
                ["zh-TW"] = "朗讀彈出提示：成就、日期和時段",
            },
            ["settingReadSongs"] = new Dictionary<string, string>
            {
                ["en"] = "Read the name of each song as it starts playing",
                ["ja"] = "曲が流れ始めたら曲名を読み上げる",
                ["ko"] = "곡이 재생될 때 곡 이름 읽기",
                ["zh-CN"] = "歌曲开始播放时朗读曲名",
                ["zh-TW"] = "歌曲開始播放時朗讀曲名",
            },
            ["repeatedN"] = new Dictionary<string, string>
            {
                ["en"] = "{0} ({1} of these)", ["ja"] = "{0}（{1}件）", ["ko"] = "{0} ({1}개)", ["zh-CN"] = "{0}（共{1}个）", ["zh-TW"] = "{0}（共{1}個）",
            },
            ["timeMorning"] = new Dictionary<string, string>
            {
                ["en"] = "Morning", ["ja"] = "朝", ["ko"] = "아침", ["zh-CN"] = "早上", ["zh-TW"] = "早上",
            },
            ["timeAfternoon"] = new Dictionary<string, string>
            {
                ["en"] = "Afternoon", ["ja"] = "昼", ["ko"] = "오후", ["zh-CN"] = "下午", ["zh-TW"] = "下午",
            },
            ["timeEvening"] = new Dictionary<string, string>
            {
                ["en"] = "Evening", ["ja"] = "夕方", ["ko"] = "저녁", ["zh-CN"] = "傍晚", ["zh-TW"] = "傍晚",
            },
            ["timeNight"] = new Dictionary<string, string>
            {
                ["en"] = "Night", ["ja"] = "夜", ["ko"] = "밤", ["zh-CN"] = "夜晚", ["zh-TW"] = "夜晚",
            },
            ["sceneLabel"] = new Dictionary<string, string>
            {
                ["en"] = "Scene description:", ["ja"] = "情景描写：", ["ko"] = "장면 설명:", ["zh-CN"] = "场景描述：", ["zh-TW"] = "場景描述：",
            },
            ["storyLabel"] = new Dictionary<string, string>
            {
                ["en"] = "Story text:", ["ja"] = "本文：", ["ko"] = "본문:", ["zh-CN"] = "正文：", ["zh-TW"] = "正文：",
            },
            ["on"] = new Dictionary<string, string>
            {
                ["en"] = "on", ["ja"] = "オン", ["ko"] = "켜짐", ["zh-CN"] = "开", ["zh-TW"] = "開",
            },
            ["off"] = new Dictionary<string, string>
            {
                ["en"] = "off", ["ja"] = "オフ", ["ko"] = "꺼짐", ["zh-CN"] = "关", ["zh-TW"] = "關",
            },
            ["selected"] = new Dictionary<string, string>
            {
                ["en"] = "selected", ["ja"] = "選択中", ["ko"] = "선택됨", ["zh-CN"] = "当前", ["zh-TW"] = "目前",
            },
            ["slotN"] = new Dictionary<string, string>
            {
                ["en"] = "Slot {0}", ["ja"] = "スロット{0}", ["ko"] = "슬롯 {0}", ["zh-CN"] = "存档位{0}", ["zh-TW"] = "存檔位{0}",
            },
            ["rollback"] = new Dictionary<string, string>
            {
                ["en"] = "Enter to go back to this line",
                ["ja"] = "Enterでこのセリフまで戻る",
                ["ko"] = "Enter를 누르면 이 대사로 돌아갑니다",
                ["zh-CN"] = "按回车返回到这一句",
                ["zh-TW"] = "按Enter返回到這一句",
            },
            ["unknownPlace"] = new Dictionary<string, string>
            {
                ["en"] = "Next story location, not named yet",
                ["ja"] = "次の物語の場所（名前はまだ不明）",
                ["ko"] = "다음 이야기 장소, 아직 이름 모름",
                ["zh-CN"] = "下一个剧情地点，尚未得知名称",
                ["zh-TW"] = "下一個劇情地點，尚未得知名稱",
            },
            ["someone"] = new Dictionary<string, string>
            {
                ["en"] = "Someone", ["ja"] = "誰か", ["ko"] = "누군가", ["zh-CN"] = "某人", ["zh-TW"] = "某人",
            },
            ["noImage"] = new Dictionary<string, string>
            {
                ["en"] = "Nothing described here.", ["ja"] = "説明できる画像はありません。", ["ko"] = "설명할 이미지가 없습니다.", ["zh-CN"] = "这里没有可描述的画面。", ["zh-TW"] = "這裡沒有可描述的畫面。",
            },
            ["tab"] = new Dictionary<string, string>
            {
                ["en"] = "tab", ["ja"] = "タブ", ["ko"] = "탭", ["zh-CN"] = "标签页", ["zh-TW"] = "分頁",
            },
            ["chosen"] = new Dictionary<string, string>
            {
                ["en"] = "chosen", ["ja"] = "選択中", ["ko"] = "선택됨", ["zh-CN"] = "已选", ["zh-TW"] = "已選",
            },
            ["notChosen"] = new Dictionary<string, string>
            {
                ["en"] = "option", ["ja"] = "選択肢", ["ko"] = "선택지", ["zh-CN"] = "选项", ["zh-TW"] = "選項",
            },
            ["currentlyX"] = new Dictionary<string, string>
            {
                ["en"] = "currently {0}", ["ja"] = "現在は{0}", ["ko"] = "현재 {0}", ["zh-CN"] = "当前为{0}", ["zh-TW"] = "目前為{0}",
            },
            ["playSample"] = new Dictionary<string, string>
            {
                ["en"] = "play a sample of this voice", ["ja"] = "この声のサンプルを再生", ["ko"] = "이 목소리 샘플 재생", ["zh-CN"] = "播放此配音的试听", ["zh-TW"] = "播放此配音的試聽",
            },
            ["tabOpened"] = new Dictionary<string, string>
            {
                ["en"] = "{0} tab.", ["ja"] = "{0}タブ。", ["ko"] = "{0} 탭.", ["zh-CN"] = "{0}标签页。", ["zh-TW"] = "{0}分頁。",
            },
            ["accessibilityButton"] = new Dictionary<string, string>
            {
                ["en"] = "Accessibility", ["ja"] = "アクセシビリティ", ["ko"] = "접근성", ["zh-CN"] = "无障碍", ["zh-TW"] = "無障礙",
            },
            ["ellipsis"] = new Dictionary<string, string>
            {
                ["en"] = "Ellipsis", ["ja"] = "三点リーダー", ["ko"] = "말줄임표", ["zh-CN"] = "省略号", ["zh-TW"] = "刪節號",
            },
            ["noText"] = new Dictionary<string, string>
            {
                ["en"] = "No text.", ["ja"] = "テキストはありません。", ["ko"] = "텍스트 없음.", ["zh-CN"] = "没有文字。", ["zh-TW"] = "沒有文字。",
            },
            ["button"] = new Dictionary<string, string>
            {
                ["en"] = "button", ["ja"] = "ボタン", ["ko"] = "버튼", ["zh-CN"] = "按钮", ["zh-TW"] = "按鈕",
            },
            ["checked"] = new Dictionary<string, string>
            {
                ["en"] = "checked", ["ja"] = "オン", ["ko"] = "선택됨", ["zh-CN"] = "已选中", ["zh-TW"] = "已選取",
            },
            ["unchecked"] = new Dictionary<string, string>
            {
                ["en"] = "not checked", ["ja"] = "オフ", ["ko"] = "선택 안 됨", ["zh-CN"] = "未选中", ["zh-TW"] = "未選取",
            },
            ["slider"] = new Dictionary<string, string>
            {
                ["en"] = "slider", ["ja"] = "スライダー", ["ko"] = "슬라이더", ["zh-CN"] = "滑块", ["zh-TW"] = "滑桿",
            },
            ["dropdown"] = new Dictionary<string, string>
            {
                ["en"] = "drop-down", ["ja"] = "ドロップダウン", ["ko"] = "드롭다운", ["zh-CN"] = "下拉列表", ["zh-TW"] = "下拉選單",
            },
            ["unavailable"] = new Dictionary<string, string>
            {
                ["en"] = "unavailable", ["ja"] = "使用不可", ["ko"] = "사용 불가", ["zh-CN"] = "不可用", ["zh-TW"] = "無法使用",
            },
        };

        /// <summary>Test hook: use this locale for the mod's own text instead of the game's.</summary>
        internal static string LocaleOverride;

        /// <summary>The game's selected locale, or "en" before Naninovel is up.</summary>
        public static string Locale
        {
            get
            {
                if (LocaleOverride != null) return LocaleOverride;
                try
                {
                    if (!Engine.Initialized) return "en";
                    return Engine.GetService<ILocalizationManager>()?.SelectedLocale ?? "en";
                }
                catch
                {
                    return "en";
                }
            }
        }

        public static string Get(string key)
        {
            if (!table.TryGetValue(key, out var byLocale)) return key;
            if (byLocale.TryGetValue(Locale, out var s)) return s;
            return byLocale["en"];
        }
    }
}
