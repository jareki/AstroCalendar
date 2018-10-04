using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroCalendar.Models
{
    static class XmlTiles
    {
        public static string GetSunXml(Sun sun)
        {
            return @"
            <tile>
              <visual branding='nameAndLogo' displayName='" + App.res.GetString("Sun")+ @"'>
                <binding template = 'TileSmall' >
                    <image src='Assets\tile-bg.png' placement='background'/>
                    <image src = 'Assets\sun.png' hint-removeMargin='true'/>
                </binding >

                <binding template='TileMedium'>
                  <image src='Assets\tile-bg.png' placement='background'/>
                  <group>
                    <subgroup hint-weight='1'>
                      <image src = 'Assets\sun.png' hint-removeMargin='true'/>
                    </subgroup>
                    <subgroup hint-weight='2' hint-textStacking='bottom'>
                      <text hint-align='center' hint-style='subtitle' >" + (sun.Result.NoDawnDusk ? "--:--" : sun.Dawn.ToString("HH:mm")) + @"</text>
                      <text hint-align='center' hint-style='subtitle'>" + (sun.Result.NoDawnDusk ? "--:--" : sun.Dusk.ToString("HH:mm")) + @"</text>
                    </subgroup>
                  </group>      
                </binding>

                <binding template = 'TileWide' >
                  <image src='Assets\tile-bg.png' placement='background'/>
                  <group >
                    <subgroup hint-weight='1'>
                      <image src = 'Assets\sun.png' hint-removeMargin='true'/>
                    </subgroup>
                    <subgroup hint-weight='3'>
                      <text hint-align='center' hint-style='title'>" + (sun.Result.NoDawnDusk ? "--:--" : sun.Dawn.ToString("HH:mm")) + @"</text>
                      <text hint-align='center' hint-style='title'>" + (sun.Result.NoDawnDusk ? "--:--" : sun.Dusk.ToString("HH:mm")) + @"</text>
                    </subgroup>
                  </group>
                </binding>

                <binding template = 'TileLarge' >
                  <image src='Assets\tile-bg.png' placement='background'/>
                  <group >
                    <subgroup hint-weight='1'></subgroup>
                    <subgroup hint-weight='2'>                      
                        <image src='Assets\sun.png' />
                    </subgroup>
                    <subgroup hint-weight='1'></subgroup>
                </group>
              <text hint-align='center' hint-style='base'>" +App.res.GetString("SunDailyDawnTimeTxt/Text")  + (sun.Result.NoDawnDusk ? "--:--" : sun.Dawn.ToString("HH:mm")) + @"</text>
              <text hint-align='center' hint-style='base'>" + App.res.GetString("SunDailyDuskTimeTxt/Text") + (sun.Result.NoDawnDusk ? "--:--" : sun.Dusk.ToString("HH:mm")) + @"</text>
              <text hint-align='center' hint-style='base'>" + App.res.GetString("SunDailyNoonTimeTxt/Text") + (sun.Result.NoDawnDusk ? "--:--" : sun.Noon.ToString("HH:mm")) + @"</text>
              <text hint-align='center' hint-style='base'>" + App.res.GetString("SunDailyLengthTimeTxt/Text") + (sun.Result.NoDawnDusk ? "--:--" : (sun.Dusk - sun.Dawn).ToString(@"hh\:mm")) + @"</text>
                 </binding>
              </visual>
            </tile>
            ";
        }

        public static string GetMoonXml(Moon moon, Sun sun)
        {
            double illumination = Astro.GetMoonPhase(moon, sun);
            var result = Astro.GetMoonPhase(illumination);
            return @"
            <tile>
              <visual branding='nameAndLogo' displayName='" + App.res.GetString("Moon") + @"'>
                <binding template = 'TileSmall' >
                    <image src='Assets\tile-bg.png' placement='background'/>
                    <image src = '" + result.Item2 + @"' hint-removeMargin='true'/>
                </binding >

                <binding template='TileMedium'>
                  <image src='Assets\tile-bg.png' placement='background'/>
                  <group>
                    <subgroup hint-weight='1'>
                      <image src = '" + result.Item2 + @"' hint-removeMargin='true'/>
                    </subgroup>
                    <subgroup hint-weight='2' hint-textStacking='bottom'>
                      <text hint-align='center' hint-style='subtitle' >" + (moon.Result.NoDawn ? "--:--" : moon.Dawn.ToString("HH:mm")) + @"</text>
                      <text hint-align='center' hint-style='subtitle'>" + (moon.Result.NoDusk ? "--:--" : moon.Dusk.ToString("HH:mm")) + @"</text>
                    </subgroup>
                  </group>      
                </binding>

                <binding template = 'TileWide' >
                  <image src='Assets\tile-bg.png' placement='background'/>
                  <group >
                    <subgroup hint-weight='1'>
                      <image src = '" + result.Item2 + @"' hint-removeMargin='true'/>
                    </subgroup>
                    <subgroup hint-weight='3'>
                      <text hint-align='center' hint-style='title'>" + (moon.Result.NoDawn ? "--:--" : moon.Dawn.ToString("HH:mm")) + @"</text>
                      <text hint-align='center' hint-style='title'>" + (moon.Result.NoDusk ? "--:--" : moon.Dusk.ToString("HH:mm")) + @"</text>
                    </subgroup>
                  </group>
                </binding>

                <binding template = 'TileLarge' >
                  <image src='Assets\tile-bg.png' placement='background'/>
                  <group >
                    <subgroup hint-weight='1'></subgroup>
                    <subgroup hint-weight='2' >
                      <image src='" + result.Item2 + @"' />
                    </subgroup>
                    <subgroup hint-weight='1'></subgroup>
                  </group>
                      <text hint-align='center' hint-style='base'>"+App.res.GetString("MoonDailyDawnTimeTxt/Text") + (moon.Result.NoDawn ? "--:--" : moon.Dawn.ToString("HH:mm")) + @"</text>
                      <text hint-align='center' hint-style='base'>" + App.res.GetString("MoonDailyDuskTimeTxt/Text") + (moon.Result.NoDusk ? "--:--" : moon.Dusk.ToString("HH:mm")) + @"</text>
                </binding>
              </visual>
            </tile>
            ";
        }
    }
}
