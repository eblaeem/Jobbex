import { animate, query, style, transition, trigger } from "@angular/animations";

export const fades =
  trigger('routeAnimations', [
    transition('* <=> *', fade())
  ]);

function fade() {
  return [
    query(':enter',
      [
        style({ opacity: 0 })
      ],
      { optional: true }
    ),
    query(':leave',
      [
        style({ opacity: 1 }),
        animate('0.2s', style({ opacity: 0 }))
      ],
      { optional: true }
    ),
    query(':enter',
      [
        style({ opacity: 0 }),
        animate('0.2s', style({ opacity: 1 }))
      ],
      { optional: true }
    )
  ];
}
